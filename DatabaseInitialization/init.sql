CREATE TABLE IF NOT EXISTS ExchangeRates (
    Id SERIAL PRIMARY KEY,
    CurrencyCode VARCHAR(3) NOT NULL,
    Rate NUMERIC(10, 4) NOT NULL,
    Date TIMESTAMP WITHOUT TIME ZONE NOT NULL
);