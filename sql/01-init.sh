

set -e

echo "[orbital-init] criando usuario aplicacional orbital_user no XEPDB1..."

sqlplus -s "system/${ORACLE_PASSWORD}@//localhost:1521/XEPDB1" <<-SQL
  WHENEVER SQLERROR EXIT SQL.SQLCODE

  CREATE USER orbital_user IDENTIFIED BY OrbitalPwd123;

  GRANT CONNECT TO orbital_user;
  GRANT RESOURCE TO orbital_user;
  GRANT CREATE SESSION TO orbital_user;
  GRANT CREATE TABLE TO orbital_user;
  GRANT CREATE SEQUENCE TO orbital_user;
  GRANT CREATE VIEW TO orbital_user;
  GRANT CREATE PROCEDURE TO orbital_user;

  ALTER USER orbital_user QUOTA UNLIMITED ON USERS;
  ALTER USER orbital_user DEFAULT TABLESPACE USERS;

  EXIT
SQL

echo "[orbital-init] orbital_user criado com sucesso."