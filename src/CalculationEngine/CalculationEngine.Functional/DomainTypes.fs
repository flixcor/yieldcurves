namespace DomainTypes

open System
open System.Text.RegularExpressions

type Date = private Date of DateTime
type FuturesContract = FuturesContract of Date

type Calendar = { weekendDays:System.DayOfWeek Set; holidays:Date Set }

type Maturity = private Maturity of float
type Tenor = { years:int; months:int; days:int; }
type Period = { startDate:Date; endDate:Date }

type DayCountConvention =
    | Actual360

type RollRule =
    | Actual
    | Following
    | Previous
    | ModifiedFollowing
    | ModifiedPrevious



module Date = 
    let value (Date d) = d.Date

    let create (dateTime:DateTime) =
        Date dateTime.Date

    let private tryCreateSingle (s:string) =
        System.DateTime.TryParse s
        //(couldParse, parsedDate)

    let tryCreate (s:string) =
        let couldParse, parsedDate = System.DateTime.TryParse(s)
        if couldParse then
            parsedDate |> create |> Ok
        else
            Error "Not valid"
    
    

    let tryCreateAll (s:string Set) =
        Set.map tryCreate s
        |> List.ofSeq
        |> Result.sequence
        |> Result.map Set.ofList



module FuturesContract =
    let tryCreate s = 
        Date.tryCreate s
        |> Result.map FuturesContract

    

    
        


module Maturity =
    let value (Maturity m) = m

    let create (d:float) =
        if d < 0.0 then
            let msg = sprintf "Maturity must not be less than 0"
            Error msg
        else
            Ok (Maturity d)


module Tenor = 
    let create t = 
        let regex s = new Regex(s)
        let pattern = regex ("(?<weeks>[0-9]+)W" + 
                                "|(?<years>[0-9]+)Y(?<months>[0-9]+)M(?<days>[0-9]+)D" +
                                "|(?<years>[0-9]+)Y(?<months>[0-9]+)M" + 
                                "|(?<months>[0-9]+)M(?<days>[0-9]+)D" +
                                "|(?<years>[0-9]+)Y" +
                                "|(?<months>[0-9]+)M" +
                                "|(?<days>[0-9]+)D")
        let m = pattern.Match(t)
        if m.Success then
              Ok{ new Tenor with 
                    years = (if m.Groups.["years"].Success 
                             then int m.Groups.["years"].Value 
                             else 0)
                    and months = (if m.Groups.["months"].Success 
                                  then int m.Groups.["months"].Value 
                                  else 0) 
                    and days = (if m.Groups.["days"].Success 
                                then int m.Groups.["days"].Value 
                                else if m.Groups.["weeks"].Success 
                                     then int m.Groups.["weeks"].Value * 7 
                                     else 0) }
        else
            Error "Invalid tenor format. Valid formats include 1Y 3M 7D 2W 1Y6M, etc"

module DayCountConvention =
    let (|InvariantEqual|_|) (str:string) arg = 
        if String.Compare(str, arg, StringComparison.OrdinalIgnoreCase) = 0 then 
            Some() 
        else 
            None

    let create dcc =
        match dcc with
            | InvariantEqual "Actual360" -> Ok Actual360
            | _ -> Error "No such DayCountConvention"


module Test =
    let offset tenor date = 
        let innerDateTime = Date.value date
        let newDateTime = innerDateTime.AddDays(float tenor.days).AddMonths(tenor.months).AddYears(tenor.years)
        Date.create newDateTime

    let dateDiff startDate endDate =
        let startDate' = Date.value startDate
        let endDate' = Date.value endDate
        startDate' - endDate'

    let dateDiffPeriod period =
        dateDiff period.startDate period.endDate

    let daysBetween startDate endDate =
        (dateDiff startDate endDate).Days

    let monthsAreEqual firstDate secondDate =
        let firstDate' = Date.value firstDate
        let secondDate' = Date.value secondDate
        firstDate'.Month = secondDate'.Month

  

    let getMaturity period dcc =
        match dcc with
            | Actual360 -> double (dateDiffPeriod period).Days / 360.0

    let rec schedule frequency period =
        seq {
            yield period.startDate
            let next = frequency period.startDate
            if (next <= period.endDate) then
                yield! schedule frequency { startDate = next; endDate = period.endDate }
        }

    let findNthWeekDay n weekDay date =
        let un = date |> Date.value
        let mutable d = new DateTime(un.Year, un.Month, 1)
        while d.DayOfWeek <> weekDay do d <- d.AddDays(1.0)
        for i = 1 to n - 1 do d <- d.AddDays(7.0)
        if d.Month = un.Month then
            d |> Date.create |> Ok
        else
            Error "No such day"    

    let semiAnnual from = 
        let un = Date.value from
        un.AddMonths(6) |> Date.create

    let isBusinessDay date calendar = 
        let un = Date.value date
        not (calendar.weekendDays.Contains un.DayOfWeek || calendar.holidays.Contains date)
    
    let addDays x date =
        date
        |> Date.value
        |> fun d -> d.AddDays(float x)
        |> Date.create
    
    let dayAfter date = 
        let un = Date.value date
        un.AddDays 1.0 |> Date.create

    let dayBefore (date:Date) = 
        let un = Date.value date
        un.AddDays -1.0 |> Date.create
    
    let deriv f x =
        let dx = (x + max (1e-6 * x) 1e-12)
        let fv = f x
        let dfv = f dx
        if (dx <= x) then
            (dfv - fv) / 1e-12
        else
            (dfv - fv) / (dx - x)
    
    // Newton's method with separate functions for f and df
    let newton f (guess:double) = 
        guess - f guess / deriv f guess
    
    // Simple recursive solver for Newton's method with separate functions for f and df, to a given accuracy
    let rec solveNewton f accuracy guess =
        let root = (newton f guess)
        if abs(root - guess) < accuracy then root else solveNewton f accuracy root
    
    // Assume Log-Linear interpolation
    let logarithmic sampleDate highDp lowDp =
        let lowDate, lowFactor = lowDp
        let highDate, highFactor = highDp
        let days1 = daysBetween sampleDate lowDate
        let days2 = daysBetween highDate lowDate

        lowFactor * 
            (highFactor / lowFactor) ** 
                (double days1 / double days2)
    
    let rec rollFollowing calendar date =
        if isBusinessDay date calendar then
            date
        else
            dayAfter date |> rollFollowing calendar

    let rec rollPrevious calendar date =
        if isBusinessDay date calendar then
            date
        else
            dayBefore date |> rollPrevious calendar

    let rec rollModifiedFollowing calendar date =
        if isBusinessDay date calendar then
            date
        else
            let next = rollFollowing calendar date
            if monthsAreEqual next date then
                next
            else
                rollPrevious calendar date

    let rec rollModifiedPrevious calendar date =
        if isBusinessDay date calendar then
            date
        else
            let prev = rollPrevious calendar date
            if monthsAreEqual prev date then
                prev
            else
                rollFollowing calendar date

    let roll rule calendar date =
        match rule with
        | RollRule.Actual -> 
            date
        | RollRule.Following -> 
            rollFollowing calendar date
        | RollRule.Previous -> 
            rollPrevious calendar date
        | RollRule.ModifiedFollowing -> 
            rollModifiedFollowing calendar date
        | RollRule.ModifiedPrevious -> 
            rollModifiedPrevious calendar date
    

    let rec rollByFollowing x calendar date =
        dayAfter date
        |> rollFollowing calendar
        |> rollByFollowing (x - 1) calendar

    let rec rollByPrevious x calendar date =
        rollPrevious calendar date
        |> dayBefore
        |> rollPrevious calendar
        |> rollByPrevious (x - 1) calendar

    let rollByModifiedFollowing x calendar date =
        let next = rollByFollowing (x - 1) calendar date
        
        // Roll the last day ModifiedFollowing
        let final = rollFollowing calendar (dayAfter next)

        if monthsAreEqual final next then
            final
        else
            rollPrevious calendar next


    let rollByModifiedPrevious x calendar date =
        // Roll n-1 days Previous
        let prev = rollByPrevious (x - 1) calendar date
                
        // Roll the last day ModifiedPrevious
        let final = rollPrevious calendar (dayAfter prev)

        if monthsAreEqual final prev then
            final
        else
            rollFollowing calendar prev
        

    let rollBy n rule calendar (date:Date) =
        match n with
        | 0 -> date
        | x -> 
            match rule with
            | RollRule.Actual -> 
                addDays x date 
            | RollRule.Following -> 
                rollByFollowing x calendar date
            | RollRule.Previous -> 
                rollByPrevious x calendar date
            | RollRule.ModifiedFollowing -> 
                rollByModifiedFollowing x calendar date
            | RollRule.ModifiedPrevious -> 
                rollByModifiedPrevious x calendar date
    
    let rec findDf interpolate sampleDate =
        function
            // exact match
            (dpDate:Date, dpFactor:double) :: tail 
                when dpDate = sampleDate
            -> dpFactor
                        
            // falls between two points - interpolate    
            | (highDate:Date, highFactor:double) :: (lowDate:Date, lowFactor:double) :: tail 
                when lowDate < sampleDate && sampleDate < highDate
            -> interpolate sampleDate (highDate, highFactor) (lowDate, lowFactor)
                  
            // recurse      
            | head :: tail -> findDf interpolate sampleDate tail
                  
            // falls outside the curve
            | [] -> failwith "Outside the bounds of the discount curve"
    
    let findPeriodDf period discountCurve = 
        let payDf = findDf logarithmic period.endDate discountCurve
        let valueDf = findDf logarithmic period.startDate discountCurve
        payDf / valueDf
    
    let computeDf dayCount fromDf toQuote =
        let dpDate, dpFactor = fromDf
        let qDate, qValue = toQuote
        (qDate, dpFactor * (1.0 / 
                            (1.0 + qValue * dayCount { startDate = dpDate; 
                                                        endDate = qDate })))
    
    // Just to compute f(guess)
    let computeSwapDf dayCount spotDate swapQuote discountCurve swapSchedule (guessDf:double) =
        let qDate, qQuote = swapQuote
        let guessDiscountCurve = (qDate, guessDf) :: discountCurve 
        let spotDf = findDf logarithmic spotDate discountCurve
        let swapDf = findPeriodDf { startDate = spotDate; endDate = qDate } guessDiscountCurve
        let swapVal =
            let rec _computeSwapDf a spotDate qQuote guessDiscountCurve =
                function
                    swapPeriod :: tail ->
                    let couponDf = findPeriodDf { startDate = spotDate; 
                                                  endDate = swapPeriod.endDate } guessDiscountCurve
                    _computeSwapDf (couponDf * (dayCount swapPeriod) * qQuote + a) 
                        spotDate qQuote guessDiscountCurve tail
    
                    | [] -> a
            _computeSwapDf -1.0 spotDate qQuote guessDiscountCurve swapSchedule
        spotDf * (swapVal + swapDf)
    
    [<Measure>] type bp
    [<Measure>] type percent
    [<Measure>] type price
            
    let convertPercentToRate (x:float<percent>) = x / 100.0<percent>
    let convertPriceToRate (x:float<price>) = (100.0<price> - x) / 100.0<price>
            
    type InterestRateQuote =
        | Rate of float
        | Percent of float<percent>
        | BasisPoints of float<bp>
        with
            member x.ToRate() =
                match x with
                | Rate r -> r
                | Percent p -> p / 100.0<percent>
                | BasisPoints bp -> bp / 10000.0<bp>
                        
            member x.ToPercentage() =
                match x with
                | Rate r -> r * 100.0<percent>
                | Percent p -> p
                | BasisPoints bp -> bp / 100.0<bp/percent>
                        
            member x.ToBasisPoints() =
                match x with
                | Rate r -> r * 10000.0<bp>
                | Percent p -> p * 100.0<bp/percent>
                | BasisPoints bp -> bp
        end
                    
    
    let contract d = Date.tryCreate d
    
    type QuoteType =
    | Overnight                     // the overnight rate (one day period)
    | TomorrowNext                  // the one day period starting "tomorrow"
    | TomorrowTomorrowNext          // the one day period starting the day after "tomorrow"
    | Cash of Tenor                 // cash deposit period in days, weeks, months
    | Futures of FuturesContract    // year and month of futures contract expiry
    | Swap of Tenor                 // swap period in years
    
    // Bootstrap the next discount factor from the previous one
    let rec bootstrap dayCount quotes discountCurve =
        match quotes with
            quote :: tail -> 
            let newDf = computeDf dayCount (List.head discountCurve) quote
            bootstrap dayCount tail (newDf :: discountCurve)
            | [] -> discountCurve
    
    // Generate the next discount factor from a fixed point on the curve 
    // (cash points are wrt to spot, not the previous df)
    let rec bootstrapCash dayCount spotDate quotes discountCurve =
        match quotes with
            quote :: tail -> 
            let spotDf = (spotDate, findDf logarithmic spotDate discountCurve)
            let newDf = computeDf dayCount spotDf quote
            bootstrapCash dayCount spotDate tail (newDf :: discountCurve)
            | [] -> discountCurve
    
    let bootstrapFutures dayCount futuresStartDate quotes discountCurve =
        match futuresStartDate with
        | Some d ->
            bootstrap dayCount
                        (Seq.toList quotes) 
                        ((d, findDf logarithmic d discountCurve) :: discountCurve)
        | None -> discountCurve
    
    // Swaps are computed from a schedule generated from spot and priced 
    // according to the curve built thusfar
    let rec bootstrapSwaps dayCount spotDate calendar swapQuotes discountCurve =
        match swapQuotes with
            (qDate, qQuote) :: tail ->
            // build the schedule for this swap                
            let swapDates = schedule semiAnnual { startDate = spotDate; endDate = qDate }
            let rolledSwapDates = Seq.map (fun (d:Date) -> roll RollRule.Following calendar d) 
                                            swapDates
            let swapPeriods = Seq.toList (Seq.map (fun (s, e) -> 
                                                    { startDate = s; 
                                                      endDate = e }) (Seq.pairwise rolledSwapDates))
                    
            // solve
            let accuracy = 1e-12
            let spotFactor = findDf logarithmic spotDate discountCurve
            let f = computeSwapDf dayCount spotDate (qDate, qQuote) discountCurve swapPeriods
            let newDf = solveNewton f accuracy spotFactor
    
            bootstrapSwaps dayCount spotDate calendar tail ((qDate, newDf) :: discountCurve)
            | [] -> discountCurve
    
    let USD = 
        let ding d = 
            { weekendDays = Set [ System.DayOfWeek.Saturday; System.DayOfWeek.Sunday ]; holidays = d}
        Set [
            "2009-01-01"; 
            "2009-01-19"; 
            "2009-02-16"; 
            "2009-05-25"; 
            "2009-07-03"; 
            "2009-09-07"; 
            "2009-10-12"; 
            "2009-11-11"; 
            "2009-11-26"; 
            "2009-12-25" ]
            |> Date.tryCreateAll
            |> Result.map ding 

        

        
            
    let curveDate = Date.tryCreate "2009-05-01"
        

    let spotDate = Result.lift2 (rollBy 2 RollRule.Following) USD curveDate
    
    let dd r a =
        (a, r)

    let tryParse c parser str value =
        parser str
        |> Result.map c
        |> Result.map (dd value)



        

    let quotes = [ (Overnight, 0.045) |> Ok;
                   (TomorrowNext, 0.045) |> Ok;
                   tryParse Cash Tenor.create "1W" 0.0462;
                   tryParse Cash Tenor.create "2W" 0.0464;
                   tryParse Cash Tenor.create "3W" 0.0465;
                   tryParse Cash Tenor.create "1M" 0.0467;
                   tryParse Cash Tenor.create "3M" 0.0493;
                   tryParse Futures FuturesContract.tryCreate "Jun2009" 95.150;
                   tryParse Futures FuturesContract.tryCreate "Jun2009" 95.150;
                   tryParse Futures FuturesContract.tryCreate "Sep2009" 95.595;
                   tryParse Futures FuturesContract.tryCreate "Dec2009" 95.795;
                   tryParse Futures FuturesContract.tryCreate "Mar2010" 95.900;
                   tryParse Futures FuturesContract.tryCreate "Jun2010" 95.910;
                   tryParse Swap Tenor.create "2Y" 0.04404;
                   tryParse Swap Tenor.create "3Y" 0.04474;
                   tryParse Swap Tenor.create "4Y" 0.04580;
                   tryParse Swap Tenor.create "5Y" 0.04686;
                   tryParse Swap Tenor.create "6Y" 0.04772;
                   tryParse Swap Tenor.create "7Y" 0.04857;
                   tryParse Swap Tenor.create "8Y" 0.04924;
                   tryParse Swap Tenor.create "9Y" 0.04983;
                   tryParse Swap Tenor.create "10Y" 0.0504;
                   tryParse Swap Tenor.create "12Y" 0.05119;
                   tryParse Swap Tenor.create "15Y" 0.05201;
                   tryParse Swap Tenor.create "20Y" 0.05276;
                   tryParse Swap Tenor.create "25Y" 0.05294;
                   tryParse Swap Tenor.create "30Y" 0.05306 ]
                   |> Result.sequence
    
    let spotPoints = quotes
                        |> Result.map 
                            (List.choose (fun (t, q) -> 
                                match t with
                                | Overnight _ -> Some (rollBy 1 RollRule.Following USD curveDate, q)
                                | TomorrowNext _ -> Some (rollBy 2 RollRule.Following USD curveDate, q)
                                | TomorrowTomorrowNext _ -> Some (rollBy 3 RollRule.Following USD curveDate, q)
                                | _ -> None))
                        |> Result.map 
                            (List.sortBy (fun (d, _) -> d))
    
    let cashPoints = quotes
                        |> List.choose (fun (t, q) -> 
                            match t with
                            | Cash c -> Some (offset c spotDate |> roll RollRule.Following USD, q)
                            | _ -> None)
                        |> List.sortBy (fun (d, _) -> d)
    
    let futuresQuotes = quotes
                        |> List.choose (fun (t, q) -> 
                            match t with
                            | Futures f -> Some (f, q)
                            | _ -> None)
                        |> List.sortBy (fun (c, _) -> c)
                                    
    let (sc, _) = List.head futuresQuotes
    let (ec, _) = futuresQuotes.[futuresQuotes.Length - 1]   
    let futuresStartDate = findNthWeekDay 3 System.DayOfWeek.Wednesday sc 
                            |> roll RollRule.ModifiedFollowing USD
    let futuresEndDate = (new DateTime(ec.Year, ec.Month, 1)).AddMonths(3)
    
    // "invent" an additional contract to capture the end of the futures schedule
    let endContract = (futuresEndDate, 0.0)
                
    let futuresPoints = Seq.append futuresQuotes [endContract]
                        |> Seq.pairwise
                        |> Seq.map (fun ((_, q1), (c2, _)) -> 
                            (findNthWeekDay 3 System.DayOfWeek.Wednesday c2 
                                |> roll RollRule.ModifiedFollowing USD, (100.0 - q1) / 100.0))
                        |> Seq.toList            
                
    let swapPoints = quotes
                        |> List.choose (fun (t, q) -> 
                            match t with
                            | Swap s -> Some (offset s spotDate |> roll RollRule.Following USD, q)
                            | _ -> None)
                        |> List.sortBy (fun (d, _) -> d)
    
    let discountFactors = 
        let actual360 period = getMaturity period Actual360
        [ (curveDate, 1.0) ]
        |> bootstrap actual360 spotPoints 
        |> bootstrapCash actual360 spotDate cashPoints
        |> bootstrapFutures actual360 (Some futuresStartDate) futuresPoints
        |> bootstrapSwaps actual360 spotDate USD swapPoints
        |> Seq.sortBy (fun (qDate, _) -> qDate)
    
    printfn "Discount Factors"
    Seq.iter (fun (d:Date, v) -> printfn "\t%s\t%.13F" (d.ToString("yyyy-MM-dd")) v) discountFactors
                
    let zeroCouponRates = discountFactors 
                          |> Seq.map (fun (d, f) 
                                        -> (d, 100.0 * -log(f) * 365.25 / double (d - curveDate).Days))
    
    printfn "Zero-Coupon Rates"
    Seq.iter (fun (d:Date, v) -> printfn "\t%s\t%.13F" (d.ToString("yyyy-MM-dd")) v) zeroCouponRates