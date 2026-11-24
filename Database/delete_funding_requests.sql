delete from requestdata where requestid in (select id from requests where type=1 or PublicationRequestId in (select id from requests where type=1))
delete from RequestHistoryLogs where requestid in (select id from requests where type=1 or PublicationRequestId in (select id from requests where type=1))
delete from RequestUsers where requestid in (select id from requests where type=1 or PublicationRequestId in (select id from requests where type=1))
delete from RequestReferees where requestid in (select id from requests where type=1 or PublicationRequestId in (select id from requests where type=1))
delete from RewardRequests where requestid in (select id from requests where type=1 or PublicationRequestId in (select id from requests where type=1))
delete from requests where PublicationRequestId in (select id from requests where type=1)
delete from requests where type=1