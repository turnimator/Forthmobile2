
400 constant far
250 constant near

1 constant rightservo
2 constant leftservo
0 constant centerservo

\ BUG: For logging, just use the FORTH words. preceded by .s

\ indices into states. States are declared near the bottom. If you add new states, add them at the end or you have to
\ renumber everything.
0 constant IDRIVING
1 constant IOBSTRUCTED
2 constant ICRASHED
3 constant IBOXED_IN
4 constant IOFF_COURSE
5 constant ITURNING_LEFT
6 constant ITURNING_RIGHT
7 constant IREVERSING


0 value desired_course

: leftSight? readlaser1 ;
: rightSight? readlaser2 ;

: freeSightLeft? readlaser1 far > ;

: tooCloseLeft? readlaser1 near < ;

: freeSightRight? readlaser2 far > ;

: tooCloseRight? readlaser2 near < ;

: freeSightAhead? readlaser0 far > ;

: freeSight? freeSightLeft? freeSightRight? freeSightAhead? and and ;

: obstacleAhead? readlaser0 near < ;

: onCourse?
	desired_course getazimuth - abs 10 < ;
;

: turnLeft
    100 speed left_bw right_fw 200 ms
;

: turnRight
    100 speed right_bw left_fw 200 ms
;

: DRIVING 
	." DRIVING "
	150 speed
	leftservo 0 servodeg rightservo 0 servodeg 0 0 servodeg 
	.s ." servos centered. Checking sides "
	\ Stay in the middle of the road
	tooCloseLeft? IF
		." Too close on the left. Turning right "
		50 right_speed 
	THEN
	tooCloseRight? IF
		." Too close on the right. Turning left "
		50 left_speed 
	THEN
	.s ." Sides checked. Gear Forward, checking for free sight ahead "
	gear_fw
	onCourse? INVERT IF
		desired_course turnto
	THEN
	freeSightAhead? IF
		150 speed
	ELSE
		100 speed
	THEN
	.s ." Checking for obstacle "
	obstacleAhead? IF
		IOBSTRUCTED
		EXIT
	THEN
	IDRIVING
;

: getOutOfHere ( -- freeAngle )
	.s ." Get out of here. Spread servos 45 deg to look to the sides. " cr
	
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
	.s ." Centering servos, looking for obstacle ahead " cr
	leftservo 0 servodeg rightservo 0 servodeg 0 0 servodeg 

	obstacleAhead? INVERT 
;

: OBSTRUCTED
	." OBSTRUCTED " CR
	." Back it up a bit "
	gear_bw
	150 speed
	.s
	getOutOfHere IF 
		IDRIVING
	ELSE
		IOBSTRUCTED
	THEN
;

: TURNING_LEFT
;

: TURNING_RIGHT
;

: CRASHED
	0 speed
	stop
	ICRASHED
; 

: BOXED_IN
	IREVERSING
;

: OFF_COURSE
	desired_course turnto
	IDRIVING
;

: REVERSING
	." REVERSING " 
	gear_bw 150 speed
	readboard get_inputs 0 = IF
		ICRASHED
	ELSE
		IOBSTRUCTED
	THEN
;

CREATE states ' DRIVING , ' OBSTRUCTED , ' CRASHED , ' BOXED_IN , ' OFF_COURSE , ' TURNING_LEFT, ' TURNING_RIGHT, ' REVERSING

: run-state ( state --) 
  cells states + @ execute ; 

: run  
getazimuth to desired_course
0 
200 0 DO
	run-state
	.s
	LOOP
	.s
	stop
;

