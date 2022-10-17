
300 constant far
150 constant near

1 constant rightservo
2 constant leftservo
0 constant centerservo

: freeSightLeft readlaser1 far > ;

: tooCloseLeft readlaser1 near < ;

: freeSightRight readlaser2 far > ;

: tooCloseRight readlaser2 near < ;

: freeSightAhead readlaser0 far > ;

: obstacleAhead readlaser0 far < ;

: turnLeft
    100 speed left_fw right_bw 200 ms
;

: turnRight
    100 speed right_fw left_bw 200 ms
;

: backItUp ( -- )

    150 speed gear_bw 
    100 0 DO
		10 ms
		readBoard getInputs 9 = INVERT LEAVE
    LOOP
    stop
;

: findFreePath ( -- )

    100 speed
    rightservo -45 servodeg
    leftservo 45 servodeg
    
	 10000 0 DO
		readLaser1 readLaser2 > 
		IF left_fw right_bw 
		ELSE right_fw left_bw
		THEN
		freeSightAhead IF
			LEAVE
		THEN
	LOOP
	servocenter
	left_fw right_fw
;


: avoidObstacle

        obstacleAhead IF
            backItUp
            findFreePath
        THEN  

        tooCloseLeft IF
            150 speed left_fw right_bw 300 ms
        THEN

        tooCloseRight IF
            150 speed left_bw right_fw 300 ms
        THEN

        
;

: drive
    150 speed
    100 0 DO
        freeSightLeft freeSightRight freeSightAhead and and IF
            150 speed gear_fw
        ELSE
            100 speed
        THEN

        freeSightLeft invert IF
            150 left_speed
            50 right_speed
            300 ms
        THEN
        freeSightRight invert IF
            150 right_speed
            50 left_speed
            300 ms
        THEN
        avoidObstacle 
        left_fw
        right_fw      
        .board
    LOOP
    stop
;
