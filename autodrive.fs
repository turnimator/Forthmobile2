
350 constant far
200 constant near

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
	leftservo 0 servodeg rightservo 0 servodeg 0 0 servodeg 
	.s ." servos centered. Checking sides "
	\ Stay in the middle of the road
	tooCloseLeft? IF
		." Too close on the left. Turning right "
		50 right_speed 
		100 left_speed 
	THEN
	tooCloseRight? IF
		." Too close on the right. Turning left "
		50 left_speed 
		100 right_speed 
	THEN
	.s ." Sides checked. Gear Forward, checking for free sight ahead "
	gear_fw
	freeSightAhead? IF
		150 speed
	ELSE
		100 speed
	THEN
	.s ." Checking for obstacle "
	obstacleAhead? IF
		IOBSTRUCTED
		.s
	ELSE
		IDRIVING
		.s
	THEN
;

: lookAround ( -- freeAngle )
	.s ." Looking around "
	rightservo -45 servodeg 
	leftservo 45 servodeg 
	freeSightLeft? IF
		." Free sight left, turning left "
		turnleft
	ELSE
		freeSightRight? IF
		." Free sight right, turning right "
			turnRight
		THEN
	THEN
	.s ." Centering servos, looking for obstacle ahead "
	leftservo 0 servodeg rightservo 0 servodeg 0 0 servodeg 

	obstacleAhead? INVERT 
;

: OBSTRUCTED
	." OBSTRUCTED" CR
	gear_bw
	150 speed
	
	lookAround IF
		IDRIVING
	ELSE
		IOBSTRUCTED
	THEN
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
100 0 DO
	run-state
	.s
	LOOP
	.s
	stop
;

