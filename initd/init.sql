CREATE TABLE metrics
(
  id SERIAL PRIMARY KEY,
  memory NUMERIC(15, 2) NOT NULL,
  proc_time NUMERIC(10, 2) NOT NULL,
  free_mem NUMERIC(15, 2) NOT NULL
);