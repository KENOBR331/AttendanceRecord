--*DataTitle 'マスタ_勤怠管理ユーザ'
--*CaptionFromComment
SELECT
    [userid]                                    -- ユーザID
    , [name]                                    -- ユーザ名
    , [gender]                                  -- 0
    , [ipaddress]                               -- ADがないため、IPアドレスで代用
    , [affiliation]                             -- 所属(部署等を数値で判別する)
    , [del_flg]                                 -- 1のときは削除とする
FROM
    [dbo].[M_user] 
WHERE
    [userid] = userid                          -- ユーザID
ORDER BY
    [userid]
