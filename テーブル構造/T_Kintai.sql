USE [kintai]  
GO  

/****** Object:  Table [dbo].[T_Kintai]    Script Date: 2025/05/18 13:30:05 ******/  
SET ANSI_NULLS ON  
GO  
  
SET QUOTED_IDENTIFIER ON  
GO  
  
CREATE TABLE [dbo].[T_Kintai](  
	[userid] [int] NOT NULL,  
	[year] [nchar](4) NOT NULL,  
	[month] [nchar](2) NOT NULL,  
	[day] [nchar](2) NOT NULL,  
	[start_time] [datetime] NULL,  
	[end_time] [datetime] NULL,  
	[trip_start_time] [datetime] NULL,  
	[trip_end_time] [datetime] NULL,  
	[trip_flg] [bit] NULL,  
	[all_day] [bit] NULL,  
	[rest_start_time] [datetime] NULL,  
	[rest_end_time] [datetime] NULL,  
	[del_flg] [bit] NULL,  
 CONSTRAINT [PK_T_Kintai] PRIMARY KEY CLUSTERED   
(  
	[userid] ASC,  
	[year] ASC,  
	[month] ASC,  
	[day] ASC  
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]  
) ON [PRIMARY]  
GO  
  
ALTER TABLE [dbo].[T_Kintai] ADD  CONSTRAINT [DF_T_Kintai_trip_flg]  DEFAULT ((0)) FOR [trip_flg]  
GO  
  
ALTER TABLE [dbo].[T_Kintai] ADD  CONSTRAINT [DF_T_Kintai_all_day]  DEFAULT ((0)) FOR [all_day]  
GO  

ALTER TABLE [dbo].[T_Kintai] ADD  CONSTRAINT [DF_T_Kintai_del_flg]  DEFAULT ((0)) FOR [del_flg]  
GO  

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'ユーザID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_Kintai', @level2type=N'COLUMN',@level2name=N'userid'  
GO  

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'年' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_Kintai', @level2type=N'COLUMN',@level2name=N'year'  
GO  

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'月(0詰めで格納)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_Kintai', @level2type=N'COLUMN',@level2name=N'month'  
GO  
  
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'日(0詰めで格納)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_Kintai', @level2type=N'COLUMN',@level2name=N'day'  
GO  
  
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'出勤時間(YYYY-MM-DDThh:mm:ss)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_Kintai', @level2type=N'COLUMN',@level2name=N'start_time'  
GO  
  
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'退勤時間(yyyy-MM-dd HH:mm:ss)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_Kintai', @level2type=N'COLUMN',@level2name=N'end_time'  
GO  
  
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'出張開始時間(yyyy-MM-dd HH:mm:ss)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_Kintai', @level2type=N'COLUMN',@level2name=N'trip_start_time'  
GO  
  
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'出張終了時間(yyyy-MM-dd HH:mm:ss)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_Kintai', @level2type=N'COLUMN',@level2name=N'trip_end_time'  
GO  
  
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'出張のときは1' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_Kintai', @level2type=N'COLUMN',@level2name=N'trip_flg'  
GO  
  
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'直帰の場合は1' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_Kintai', @level2type=N'COLUMN',@level2name=N'all_day'  
GO  
  
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'休憩開始時間(yyyy-MM-dd HH:mm:ss)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_Kintai', @level2type=N'COLUMN',@level2name=N'rest_start_time'  
GO  
  
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'休憩終了時間(yyyy-MM-dd HH:mm:ss)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_Kintai', @level2type=N'COLUMN',@level2name=N'rest_end_time'  
GO  
  
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'削除フラグ(1で論理削除)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_Kintai', @level2type=N'COLUMN',@level2name=N'del_flg'  
GO  
  
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'勤怠テーブル' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'T_Kintai'  
GO  
  
  
  
