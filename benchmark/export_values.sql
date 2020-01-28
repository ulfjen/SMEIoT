COPY (SELECT sv.value, extract(epoch from sv.created_at), s.name FROM sensor_values sv JOIN sensors s ON sv.sensor_id = s.id WHERE s.id >= 45) To '/tmp/dump-0128.csv' WITH CSV HEADER DELIMITER ',';
