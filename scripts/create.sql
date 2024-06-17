CREATE TABLE TESTS (
    test_id SERIAL PRIMARY KEY,
    test_result VARCHAR(10) NOT NULL CHECK (test_result IN ('passed', 'failed')),
    error_description TEXT,
    start_time TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);
CREATE TABLE OPERATIONS (
    start_timestamp TIMESTAMPTZ NOT NULL,
    end_timestamp TIMESTAMPTZ NOT NULL,
    duration_ms INT NOT NULL,
    operation_code VARCHAR(50) NOT NULL CHECK (operation_code IN ('opening', 'catalog', 'all', 'apple', 'iphone', 'element', 'add-to-cart', 'go-to-cart-and-check', 'cookie')),
    operation_result VARCHAR(10) NOT NULL CHECK (operation_result IN ('passed', 'failed')),
    error_description TEXT,
    test_id INT
);
