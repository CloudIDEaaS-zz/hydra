set TargetDir=%HYDRASOLUTIONPATH%\ApplicationGenerator\TestOutput\contoso.Web\contoso

rmdir /S/Q "%TargetDir%"
if exist "%TargetDir%" rd /s /q "%TargetDir%"