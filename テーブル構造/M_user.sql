--*DataTitle '�}�X�^_�ΑӊǗ����[�U'
--*CaptionFromComment
SELECT
    [userid]                                    -- ���[�UID
    , [name]                                    -- ���[�U��
    , [gender]                                  -- 0
    , [ipaddress]                               -- AD���Ȃ����߁AIP�A�h���X�ő�p
    , [affiliation]                             -- ����(�������𐔒l�Ŕ��ʂ���)
    , [del_flg]                                 -- 1�̂Ƃ��͍폜�Ƃ���
FROM
    [dbo].[M_user] 
WHERE
    [userid] = userid                          -- ���[�UID
ORDER BY
    [userid]
