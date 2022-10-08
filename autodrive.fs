
: freeSightLeft readlaser1 v_left_speed 50 + > ;

: freeSightRight readlaser2 50 v_right_speed 50 + > ;

: freeSightAhead readlaser0 v_speed_left v_speed_right 50 + + > ;

: turnLeft
    100 speed left_fw right_bw 100 ms
;

: turnRight
    100 speed right_fw left_bw 100 ms
;

: backItUp
    150 speed gear_bw 500 ms
;

: avoidObstacle
    backItUp
    readLaser1 readLaser2 < IF \ 
        turnRight
    ELSE
        turnLeft
    THEN


;

: drive
    150 speed
    200 0 DO
        freeSightLeft freeSightRight freeSightAhead and and IF
            speed 200 gear_fw
        THEN
        freeSightLeft invert IF
            150 left_speed
            50 right_speed
        THEN
        freeSightRight invert IF
            150 right_speed
            50 left_speed
        THEN
        freeSightAhead invert IF
            avoidObstacle
        THEN
        .board
    LOOP
    stop
;

