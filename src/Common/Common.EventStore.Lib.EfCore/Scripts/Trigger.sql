CREATE OR REPLACE FUNCTION notify_new_event()
RETURNS trigger AS $$
BEGIN
  PERFORM pg_notify(
    'new_event',
    row_to_json(NEW)::text
  );

  RETURN NEW;
END;
$$ LANGUAGE plpgsql;


CREATE TRIGGER new_event
AFTER INSERT
ON events
FOR EACH ROW
EXECUTE PROCEDURE notify_new_event()