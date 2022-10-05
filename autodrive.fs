

: freeSightLeft
    readlaser1
    100 > IF
        -1
    ELSE
        0
    THEN
;

: freeSightRight
    readlaser2
    100 > IF
        -1
    ELSE
        0
    THEN
;

: freeSightAhead
    readlaser0
    400 > IF
        -1
    ELSE
        0
    THEN
;

