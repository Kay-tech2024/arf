Use this command to run large sql files:

sqlcmd -S .\SQLEXPRESS -d master -i db_prod_2017_01_18.sql -f 65001

sqlcmd -b -S .\SQLEXPRESS -d ecs -i ClaimMigration.sql -f 65001
sqlcmd -b -S .\ECS -d ecs-prod -i ClaimMigration.sql -f 65001

To set password for all users to 123456:

update users set isactivated=1, IsDeleted=0, phonenumberconfirmed=1, emailconfirmed=1, passwordhash = 'AFDn7WXfSXvvQ9X29S0t/tqKGG8lS7To/E7zVj7mbaxOoBmbN2xE6JwwPQTz/Td8Fg==', ForcePasswordChange=0, AccessFailedCount=0, LockoutEnabled=0

sqlcmd -b -S .\ECS -d ecs-prod -i moe_data_2017_08_27.sql -f 65001