



2 constant LED_BUILTIN

\ 74165 Parallel to serial
18 constant P2S_CE	\ set high when P2S_SCK is HIGH to disable clock
19 constant P2S_SHLD	\ Shift=1, load=0
17 constant   P2S_SCK
23 constant P2S_SDA

P2S_SHLD output pinMode
P2S_CE output pinMode
P2S_SCK output pinMode
P2S_SDA input pinMode

\ 74595 Serial to parallel
4 constant S2P_SDA
16 constant S2P_LATCH	\ aka RCLK
17 constant S2P_SCL


S2P_SCL output pinMode
S2P_SDA output pinMode
S2P_LATCH output pinMode


\ Motor control
15 constant M_PWMA
5 constant M_PWMB
\ LSB 0-3 of S2P give IN1-IN4 on the L298 motor controller connected to J15

also ledc

0 5000 8 ledcsetup drop
1 5000 8 ledcsetup drop

M_PWMA 0 ledcattachPin
M_PWMB 1 ledcattachPin

21 constant I2C_SDA
22 constant I2C_SCL

13 constant CNT_RESET \ Reset the pulse counters on J21 and J22

CNT_RESET output pinMode
CNT_RESET 0 digitalWrite 0 ms
CNT_RESET 1 digitalWrite 0 ms
CNT_RESET 0 digitalWrite

\ VL53L0X uses I2C. To initialize, only keep one of these high and the 
\ others LOW until the addresses are set. When the I2C address is set, all of these should be HIGH
14 constant XSHUT1
27 constant XSHUT2
26 constant XSHUT3


LED_BUILTIN output pinMode 
: blinkHello
  0 DO
    LED_BUILTIN 0 digitalwrite
    200 ms
    LED_BUILTIN 1 digitalwrite
    200 ms
  LOOP
;

: reset_counter
  CNT_RESET 1 digitalWrite
  20 ms
  CNT_RESET 0 digitalWrite
;

1 blinkHello

\ READ PISO REGISTERS

: readP2S ( - u u u ) \ read all three registters, leaving the contents on the stack
   decimal
  P2S_SHLD 0 digitalWrite 0 ms \ load data
  P2S_SHLD 1 digitalWrite 0 ms \ prepare to shift
  P2S_SCK 1 digitalWrite
  P2S_CE 0 digitalWrite
  P2S_SCK 0 digitalWrite

  0
  8 0 DO
    1 LSHIFT
    P2S_SCK 0 digitalWrite
    P2S_SDA digitalRead OR
    P2S_SCK 1 digitalWrite
  LOOP
  ~
  0
  8 0 DO
    1 LSHIFT
    P2S_SCK 0 digitalWrite
    P2S_SDA digitalRead OR
    P2S_SCK 1 digitalWrite
  LOOP
  ~
  0
  8 0 DO
    1 LSHIFT
    P2S_SCK 0 digitalWrite
    P2S_SDA digitalRead OR
    P2S_SCK 1 digitalWrite
  LOOP
  ~
;


: writeS2P ( u u --)
    decimal
    S2P_LATCH 0 digitalWrite
    8 0 DO
        dup 1 and 
        S2P_SCL 0 digitalWrite
        S2P_SDA swap digitalWrite
        S2P_SCL 1 digitalWrite
        1 RSHIFT
    LOOP
    drop
   8 0 DO
        dup 1 and 
        S2P_SCL 0 digitalWrite
        S2P_SDA swap digitalWrite
        S2P_SCL 1 digitalWrite
        1 RSHIFT
    LOOP
    drop
    S2P_LATCH 1 digitalWrite
;

: showBoard
  readp2s
    binary
   ." Inputs        "  ~ . cr
    decimal
   ." Left wheel  " . cr
  ." Right wheel "  . cr

;


0 value outreg1
0 value outreg2

 : writeBoard
  outreg1 outreg2 writes2p
;


 decimal 

: outputOn ( u -- )
   decimal
   dup 8 < IF
    1 swap lshift outreg1 or to outreg1 
  ELSE
    1 swap 8 - lshift outreg2 or to outreg2 
  THEN
;

: outputOff ( u -- )
   decimal
   dup 8 < IF
    1 swap lshift ~ outreg1 and to outreg1 
  ELSE
    1 swap 8 - lshift ~ outreg2 and to outreg2 
  THEN
;

: outputClear
 0 to outreg1
 0 to outreg2
;

 decimal 

: .regs
  binary
 ." outreg1 " outreg1 . cr 
 ." outreg2 " outreg2 . cr 
 decimal
; 



: blinkoutputs
   decimal
  16 0 DO
    i outputon writeboard
    100 ms
    i outputoff writeboard
    100 ms
  LOOP
;

blinkoutputs


: showboard2
    readP2s
    binary .  
    decimal . . cr 
; 

: left_speed ( u -- )
    0 swap ledcwrite 
;

: right_speed ( uspeed -- )
    1 swap ledcwrite 
;

: speed ( uspeed --)
  dup 1 swap ledcwrite 0 swap ledcwrite
 ;

: don
  outputon
  writeboard
;

: doff
  outputoff
  writeboard
;

: left_forward
   6 doff 7 don
;

: left_backward
    7 doff 6 don
;

: right_backward
    4 doff 5 don
;

: right_forward
    5 doff 4 don
;

: stop
    4 outputoff 5 outputoff 6 outputoff 7 outputoff
    writeboard
;

: gear_forward
    left_forward right_forward
;

: gear_backward 
    left_backward right_backward
;

setupservo
setuplaser

