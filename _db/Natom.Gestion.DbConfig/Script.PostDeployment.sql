--dbo.Config----------------------------------------------------------------------------------------
----------------------------------------------------------------------------------------------------
set nocount on;
;with cte_data([Clave],[Valor],[Description])
as (select * from (values
--//////////////////////////////////////////////////////////////////////////////////////////////////
('General.ProductName', 'Gestión', 'Nombre del producto en General. Aplica para el envio de mails por registro y recupero de clave'),
('Logging.Discord.WebhookInvoker.CancellationTokenDurationMS', '5000', 'Duración del CancellationToken del llamado Http a Discord.'),
('Logging.Discord.EnableLog', 'True', 'Habilita / Deshabilita el Log en el servicio de Discord.'),
('Logging.Discord.Info.EnableLog', 'True', 'Habilita / Deshabilita el Log para el tipo ''Info'' en el servicio de Discord.'),
('Logging.Discord.Info.WebhookUrl', 'https://discord.com/api/webhooks/959235466056581140/a6TWiyfppY9izr2o1RUTqHxJO8BOaW72Y6Ce3BKOPRUMTDQmGIae-hcn4p-3szZuEFyQ', 'URL del Webhook de Discord para avisos ''Info''.'),
('Logging.Discord.ServiceStatus.EnableLog', 'True', 'Habilita / Deshabilita el Log para el tipo ''ServiceStatus'' en el servicio de Discord.'),
('Logging.Discord.ServiceStatus.WebhookUrl', 'https://discord.com/api/webhooks/959235622860636180/HJJ-qjKMcqn8-4DRiNFC2aqf14hrsYfCfFVxiOGgYesgPiD0k9hwh6jx817Wvwq9UEQb', 'URL del Webhook de Discord para avisos ''ServiceStatus''.'),
('Logging.Discord.Exception.EnableLog', 'True', 'Habilita / Deshabilita el Log para el tipo ''Exception'' en el servicio de Discord.'),
('Logging.Discord.Exception.WebhookUrl', 'https://discord.com/api/webhooks/959235189836480513/n8GswHPWW7QlYGF9lxiFDyf_pDCiVho77SkLTMrt5Pjw7zTSEQVUW3CJYSFUYqU-EiBN', 'URL del Webhook de Discord para avisos ''Exception''.'),
('WebApp.Admin.URL','http://localhost:4202','URL de acceso a la aplicación de Administradores'),
('WebApp.Admin.Authentication.TokenDurationMins', '120', 'Duración del token de seguridad (en minutos) para la aplicación de Administradores'),
('WebApp.Admin.Authentication.Admin.UserName', 'admin', 'Username del usuario -admin- para la aplicación de Administradores'),
('WebApp.Admin.Authentication.Admin.Password', 'Azsxdc123', 'Password del usuario -admin- para la aplicación de Administradores'),
('WebApp.Admin.NewDeployment.SqlPackageEXE.Path', 'C:\\Program Files\\Microsoft SQL Server\\160\\DAC\\bin\\SqlPackage.exe', 'Path de sqlpackage.exe'),
('WebApp.Admin.NewDeployment.DACPAC.Path', '_dacpacs//Natom.Gestion.Dbz000.dacpac', 'Path del DACPAC a desplegar'),
('WebApp.Admin.NewDeployment.NewDB.Name', 'Gestion_zXXX', 'Nombre de la nueva base de datos a crear'),
('WebApp.Admin.NewDeployment.TargetServerName', 'localhost', 'Nombre del servidor SQL donde se realizará el despliegue'),
('WebApp.Clientes.URL','http://localhost:4201','URL de acceso a la aplicación de Clientes'),
('WebApp.Clientes.Authentication.TokenDurationMins', '1440', 'Duración del token de seguridad (en minutos) para la aplicación de Clientes'),
('WebApp.Clientes.Authentication.Admin.UserName', 'admin', 'Username del usuario -admin- para la aplicación de Clientes'),
('WebApp.Clientes.Authentication.Admin.Password', 'Azsxdc123', 'Password del usuario -admin- para la aplicación de Clientes'),
('ConnectionStrings.DbLogs', 'Data Source=localhost; Initial Catalog=Gestion_Logs; Integrated Security=SSPI;', 'ConnectionString de la base de datos de Logs'),
('ConnectionStrings.DbSecurity', 'Data Source=localhost; Initial Catalog=Gestion_Security; Integrated Security=SSPI;', 'ConnectionString de la base de datos de Security'),
('ConnectionStrings.DbMaster', 'Data Source=localhost; Initial Catalog=Gestion_Master; Integrated Security=SSPI;', 'ConnectionString de la base de datos de Master'),
('ConnectionStrings.DbzXXX', 'Data Source=localhost; Initial Catalog=Gestion_zXXX; Integrated Security=SSPI;', 'ConnectionString de la base de datos de cada cliente'),
('Cache.RedisServer.IP','localhost','IP del servidor Redis'),
('Cache.RedisServer.Port','6379','Puerto del servidor Redis'),
('Cache.RedisServer.Ssl','False','Si utilizar o no SSL en la conexión a Redis'),
('Cache.RedisServer.KeepAlive','30','Configuración de Redis'),
('Cache.RedisServer.ConnectTimeout','15000','Configuración de Redis'),
('Cache.RedisServer.SyncTimeout','15000','Configuración de Redis'),
('RabbitMQ.EnabbleSSL', 'False', 'Si utilizar o no SSL en la conexión a RabbitMQ'),
('Mailing.SenderName', 'Natom Gestión', 'Servicio de mailing: Nombre fantasia emisor'),
('Mailing.SMTP.User', 'no-reply@w1362013.ferozo.com', 'Servicio de mailing: Usuario SMTP'),
('Mailing.SMTP.Password', '6uC/Vz78qJ', 'Servicio de mailing: Clave del usuario SMTP'),
('Mailing.SMTP.Host', 'mail.w1362013.ferozo.com', 'Servicio de mailing: Dirección Host SMTP'),
('Mailing.SMTP.Port', '587', 'Servicio de mailing: Puerto Host SMTP'),
('Mailing.SMTP.EnableSSL', 'False', 'Servicio de mailing: Usar SMTP con SSL')
--\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
)c([Clave],[Valor],[Description]))
merge	[dbo].[Config] as t
using	cte_data as s
on		1=1 and t.[Clave] = s.[Clave]
when matched then
	update set
	[Valor] = s.[Valor],[Description] = s.[Description]
when not matched by target then
	insert([Clave],[Valor],[Description])
	values(s.[Clave],s.[Valor],s.[Description])
when not matched by source then
	delete;