
: freeSightLeft readlaser1 200 > ;

: freeSightRight readlaser2 200 > ;

: freeSightAhead readlaser0 200 > ;

: drive
    100 0 DO
        freeSightLeft freeSightRight freeSightAhead and and IF
            speed 200 gear_fw
        THEN
        freeSightLeft invert IF
            0 right_speed
        THEN
        freeSightRight invert IF
            0 left_speed
        THEN
        freeSightAhead invert IF
            left_bw 200 speed 1000 ms stop
        THEN
    LOOP
    stop
;

