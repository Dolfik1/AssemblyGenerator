function run($file, $output) {
    ildasm /out=$output /utf8 /tokens /metadata=mdheader /metadata=validate $file
}

run $PSScriptRoot\..\src\AssemblyGenerator.SampleAssembly\bin\Debug\netstandard2.0\AssemblyGenerator.SampleAssembly.dll $PSScriptRoot\..\expected.il
run $PSScriptRoot\..\src\AssemblyGenerator.Example\output.dll $PSScriptRoot\..\actual.il
