
: freeSightLeft readlaser1 v_left_speed 50 + > ;

: tooCloseLeft readlaser1 100 < ;

: freeSightRight readlaser2 v_right_speed 50 + > ;

: tooCloseRight readlaser2 100 < ;

: freeSightAhead readlaser0 v_left_speed v_right_speed 50 + + > ;

: obstacleAhead readlaser0 200 < ;

: turnLeft
    100 speed left_fw right_bw 100 ms
;

: turnRight
    100 speed right_fw left_bw 100 ms
;

: backItUp

    150 speed gear_bw 500 ms stop
;

: avoidObstacle

        obstacleAhead IF
            backItUp
        THEN  

        tooCloseLeft IF
            speed 150 right_bw 200 ms
        THEN

        tooCloseRight IF
            speed 150 left_bw 200 ms
        THEN
;

: drive
    150 speed
    100 0 DO
        freeSightLeft freeSightRight freeSightAhead and and IF
            speed 150 gear_fw
        ELSE
            speed 100
        THEN
        avoidObstacle
        freeSightLeft invert IF
            150 left_speed
            50 right_speed
            100 ms
        THEN
        freeSightRight invert IF
            150 right_speed
            50 left_speed
            100 ms
        THEN
       
        .board
    LOOP
    stop
;

