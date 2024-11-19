CREATE TABLE IF NOT EXISTS ExchangeRates (
    Id SERIAL PRIMARY KEY,
    Currency VARCHAR(3) NOT NULL,
    Rate NUMERIC(10, 4) NOT NULL,
    Date DATE NOT NULL
);