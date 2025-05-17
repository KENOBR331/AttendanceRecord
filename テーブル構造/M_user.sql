--*DataTitle 'マスタ_勤怠管理ユーザ'
--*CaptionFromComment
create table [dbo].[M_user] (
  [userid] int not null
  , [name] nvarchar(30) not null
  , [gender] bit
  , [ipaddress] nchar(15) not null
  , [affiliation] int
  , [del_flg] bit default 0
  , primary key (userid)
);
