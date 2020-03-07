use meta;
go

drop procedure if exists sp_log;
go

create procedure sp_log
(
    @job_id     uniqueidentifier,
    @job_name   varchar(100)  ='undefined',
    @type       varchar(100)  = null,
    @component  varchar(100)  = null,
    @message    varchar(1000) = null
)
with execute as caller
as
begin

    insert meta.dbo.applog
    (
	    job_id,
	    job_name,
        type,
        component,
        message,
        time
    )
    select
	    @job_id,
	    @job_name,
        @type,
        @component,
        @message,
	    getdate()

end
go

if 1=0
begin
    print 'unit test';
    --truncate table meta.dbo.applog

    declare
        @job_id     uniqueidentifier,
        @job_name   varchar(100),
        @type       varchar(100),
        @component  varchar(100),
        @message    varchar(1000);

    select
        @job_id     = newid(),
        @job_name   = 'Unit-Test',
        @type       = 'error',
        @component  = 'unitest',
        @message    = formatmessage('Now is the %s for all %s men to come to the %s of their %s!',
            'time', 'young', 'aid', 'country');

    exec meta.dbo.sp_log
        @job_id,
        @job_name,
        @type,
        @component,
        @message;

    select * from meta.dbo.applog;
end
go
