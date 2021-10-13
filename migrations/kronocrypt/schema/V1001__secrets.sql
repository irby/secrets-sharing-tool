CREATE TABLE IF NOT EXISTS Secrets (
    PRIMARY KEY (id),
    id VARCHAR(64) NOT NULL,
    message BLOB NULL,
    encryptedsymmetrickey BLOB NULL,
    iv BLOB NULL,
    signedhash BLOB NULL,
    expiredatetime TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    numberofattempts INT NOT NULL,
    createdon TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    createdby VARCHAR(30) NULL,
    modifiedon TIMESTAMP NULL,
    modifiedby VARCHAR(30) NULL,
    isactive BIT NOT NULL
);