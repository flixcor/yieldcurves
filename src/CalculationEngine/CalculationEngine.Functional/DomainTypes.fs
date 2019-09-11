namespace DomainTypes

open System
open System.Text.RegularExpressions

type Date = private Date of DateTime


type Maturity = private Maturity of float
type Tenor = { years:int; months:int; days:int; }
type Period = { startDate:Date; endDate:Date }

type DayCountConvention =
    |Actual360

module Date = 
    let value (Date d) = d.Date

    let create (dateTime:DateTime) =
        Date dateTime.Date

    
        


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
            | _ -> Error "NOT GOOD ENOUGH!"


module Test =
    let offset tenor (date:Date) = 
        let innerDateTime = Date.value date
        let newDateTime = innerDateTime.AddDays(float tenor.days).AddMonths(tenor.months).AddYears(tenor.years)
        Date.create newDateTime

    
    let dateDiff period =
        let startDate = Date.value period.startDate
        let endDate = Date.value period.endDate
        startDate - endDate


    let getMaturity period (dcc:DayCountConvention) =
        match dcc with
            | Actual360 -> double (dateDiff period).Days / 360.0

    let rec schedule frequency period =
        seq {
            yield period.startDate
            let next = frequency period.startDate
            if (next <= period.endDate) then
                yield! schedule frequency { startDate = next; endDate = period.endDate }
        }