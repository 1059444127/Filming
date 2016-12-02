%~d0
cd %~p0

protoc.exe --cpp_out=. FilmingJob.proto

copy /Y *.h .\include\*.h
copy /Y *.cc .\src\*.cc

