%~d0
cd %~p0

protoGen.exe --proto_path=. FilmingJob.proto

copy /Y *.cs ..\UIH.Mcsf.Filming.Utility\*.cs