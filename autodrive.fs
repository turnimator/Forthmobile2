
: freeSightLeft readlaser1 170 > ;

: freeSightRight readlaser2 170 > ;

: freeSightAhead readlaser0 170 > ;

: turnLeft
    100 speed left_fw right_bw 500 ms
;

: turnRight
    100 speed right_fw left_bw 500 ms
;

: backItUp
    100 speed gear_bw 500 ms
;

: avoidObstacle
    backItUp

    freeSightRight invert freesightLeft and IF
        turnLeft
        exit
    THEN
    freeSightLeft invert freesightRight and IF
        turnRight
        exit
    THEN
    freeSightLeft IF
        turnLeft
        exit
    THEN
    FreeSightRight IF
        turnRight
    THEN

;

: drive
    150 speed
    200 0 DO
        freeSightLeft freeSightRight freeSightAhead and and IF
            speed 200 gear_fw
        THEN
        freeSightLeft invert IF
            50 right_speed
        THEN
        freeSightRight invert IF
            50 left_speed
        THEN
        freeSightAhead invert IF
            avoidObstacle
        THEN
        .board
    LOOP
    stop
;

