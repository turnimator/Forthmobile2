
300 constant far
100 constant near

1 constant rightservo
2 constant leftservo
0 constant centerservo

\ indices into states
0 constant IDRIVING
1 constant IOBSTRUCTED
2 constant ICRASHED
3 constant IBOXED_IN
4 constant IOFF_COURSE


: leftSight? readlaser1 ;
: rightSight? readlaser2 ;

: freeSightLeft? readlaser1 far > ;

: tooCloseLeft? readlaser1 near < ;

: freeSightRight? readlaser2 far > ;

: tooCloseRight? readlaser2 near < ;

: freeSightAhead? readlaser0 far > ;

: freeSight? freeSightLeft? freeSightRight? freeSightAhead? and and ;

: obstacleAhead? readlaser0 near < ;

: turnLeft
    100 speed left_bw right_fw 200 ms
;

: turnRight
    100 speed right_bw left_fw 200 ms
;

: DRIVING 
	." driving ... "
	servocenter
	obstacleAhead? IF
		IOBSTRUCTED
		EXIT
	THEN
	\ Stay in the middle of the road
	rightSight? leftSight? > IF
		75 right_speed 
		100 left_speed 
	THEN
	rightSight? leftSight? < IF
		75 left_speed 
		100 right_speed 
	THEN
	gear_fw
	freeSightAhead? IF
		200 speed
	ELSE
		100 speed
	THEN
	IDRIVING
;

: lookAround ( -- freeAngle )
	leftservo 45 servodeg freeSightLeft? IF
		servocenter
		turnleft
		IDRIVING
		EXIT
	THEN
	rightservo -45 servodeg freeSightRight? IF
		servocenter
		turnRight
		IDRIVING
		EXIT
	THEN
	gear_bw
	100 speed 300 ms
	IOBSTRUCTED
;

: OBSTRUCTED
	." OBSTRUCTED" CR
	STOP
	lookAround
	IOBSTRUCTED
;

: CRASHED
	0 speed
	stop
	ICRASHED
; 

: BOXED_IN
	IOBSTRUCTED
;

: OFF_COURSE
	IDRIVING
;

CREATE states ' DRIVING , ' OBSTRUCTED , ' CRASHED , ' BOXED_IN , OFF_COURSE ,

: run-state ( state --) 
  cells states + @ execute ; 

: run  
0 
10 0 DO
	run-state
	.s
	LOOP
	.s
;

