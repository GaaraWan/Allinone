@echo off

set EzOcrEnvir=ocr


:: (1) 尋訪所有的 conda envs
for /f "tokens=1" %%i in ('conda info --envs') do (
    if "%%i" == "%EzOcrEnvir%" (
        @REM echo CUDA
        @REM set isEzSetEnvirOK=true
        goto :RUN_
    )
)

:: (2) 如果以上兩個虛擬環境都不存在，顯示警告訊息
echo *
echo * [Error] %EzOcrEnvir% environment does NOT exist !
echo *
goto :END_

:RUN_
@REM call C:\Anaconda3\Scripts\activate.bat %envFolder%
call conda activate %EzOcrEnvir%

run_ocr_server

:: 最終一定要用 exit, 才能退出 command shell
:END_
exit