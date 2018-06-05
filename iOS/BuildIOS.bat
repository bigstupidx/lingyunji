cd %~dp0
set self_path=%~dp0
set unity_path = %1
set certificate="ios_development.cer:AppleIncRootCertificate.cer:AppleWWDRCA.cer:my_key.key:123"
set provision="%self_path%/iOS.mobileprovision"
set name=%2
"%IOSUNITYBUILDER_PATH%\build.cmd" %unity_path% -identity %certificate% -provision %provision% -name %name% -ipa -archs "arm64"