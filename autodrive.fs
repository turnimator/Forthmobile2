
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
5 constant IREVERSING


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

: badlyOffCourse?
	desired_course getazimuth - abs 100 > ;
;

: turnLeft
    100 speed left_bw right_fw 200 ms
    getazimuth getazimuth DROP DROP
;

: turnRight
    100 speed right_bw left_fw 200 ms
    getazimuth getazimuth DROP DROP
;

: correct_course
	." Correcting course. Desired Actual "
	desired_course dup . getazimuth dup . - dup
	." Deviation " . 
	dup 20 > IF
		." Turning right "
		right_speed? 20 - 0 min right_speed
	THEN
	-20 < IF
	." Turning left "
		left_speed? 20 - 0 min left_speed
	THEN
	." Course adjusted" cr
;

: DRIVING 
	cr .s ." DRIVING "
	150 speed
	getazimuth .
	correct_course
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
	freeSightAhead? IF
		150 speed
	ELSE
		100 speed
	THEN
	.s ." Checking for obstacle "
	obstacleAhead? IF
		." Obstacle detected" 
		IOBSTRUCTED
	ELSE
		." No obstacle. Checking course."
		\ If there are no obstacles, we can check for correct course
		onCourse? INVERT IF
			." Off course"
			IOFF_COURSE
		ELSE
			." On course. Driving ... "
			IDRIVING
		THEN
	THEN
;

: getOutOfHere ( -- f )
	.s ." getOutOfHere Spread servos 45 deg to look to the sides. " cr
	
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
	cr .s ." OBSTRUCTED " 
	." Backing it up a bit " 
	gear_bw
	150 speed 500 ms
	
	getOutOfHere IF 
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
	IREVERSING
;

: OFF_COURSE
	cr .s ." OFF_COURSE "
	
	freeSightAhead? INVERT IF
		gear_bw 150 speed 300 ms stop
	THEN
	\ If we get back on course, get back to driving
	correct_course desired_course getazimuth - abs ms  \ Try to make the correction depend on the amount of deviation
	badlyOffCourse? IF
		IOFF_COURSE
	ELSE
		IDRIVING
	THEN

;

: REVERSING
	cr .s ." REVERSING " 
	gear_bw 150 speed
	cr
	readboard get_inputs 0 = IF
		ICRASHED
	ELSE
		IOBSTRUCTED
	THEN
;

CREATE states ' DRIVING , ' OBSTRUCTED , ' CRASHED , ' BOXED_IN , ' OFF_COURSE , ' REVERSING , 

0 value vend_run
0 value vstate

: run-state ( state --) 
  cells states + @ execute ; 
  getazimuth DROP
  getazimuth DROP
  getazimuth DROP
  
	." Orientation:" getazimuth . ." Type X run to run for X seconds."

: run ( uhowlong -- )
	1000 * ms-ticks + to vend_run
	getazimuth .
	getazimuth .
	getazimuth .
	getazimuth .
	getazimuth .
	getazimuth .
	getazimuth to desired_course
	IDRIVING
	BEGIN
		run-state
		.s
	vend_run ms-ticks < UNTIL
	.s
	stop
	." END OF RUN" CR
;

