USE [kintai]  
GO  
  
/****** Object:  Table [dbo].[M_user]    Script Date: 2025/05/18 13:33:22 ******/  
SET ANSI_NULLS ON  
GO  
  
SET QUOTED_IDENTIFIER ON  
GO  
  
CREATE TABLE [dbo].[M_user](  
	[userid] [int] NOT NULL,  
	[name] [nvarchar](30) NOT NULL,  
	[gender] [bit] NULL,  
	[ipaddress] [nchar](15) NOT NULL,  
	[affiliation] [int] NULL,  
	[del_flg] [bit] NULL,  
 CONSTRAINT [PK_M_user] PRIMARY KEY CLUSTERED   
(  
	[userid] ASC  
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]  
) ON [PRIMARY]  
GO  
  
ALTER TABLE [dbo].[M_user] ADD  CONSTRAINT [DF_M_user_del_flg]  DEFAULT ((0)) FOR [del_flg]  
GO  
  
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ユーザID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'M_user', @level2type=N'COLUMN',@level2name=N'userid'  
GO  
  
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ユーザ名' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'M_user', @level2type=N'COLUMN',@level2name=N'name'  
GO  
  
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'0:男性 1:女性' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'M_user', @level2type=N'COLUMN',@level2name=N'gender'  
GO  
  
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ADがないため、IPアドレスで代用' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'M_user', @level2type=N'COLUMN',@level2name=N'ipaddress'  
GO  
  
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'所属(部署等を数値で判別する)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'M_user', @level2type=N'COLUMN',@level2name=N'affiliation'  
GO  
  
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'1のときは削除とする' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'M_user', @level2type=N'COLUMN',@level2name=N'del_flg'  
GO  
  
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'マスタ_勤怠管理ユーザ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'M_user'  
GO  


