# dotnet.exe test has problems with these multi-target dlls.
# Enumerate test dlls here and run tests with this script

$testFixtures = @('bin\Debug\Engine\net461\Engine.dll',
                 'bin\Debug\EntityComponentSystemCSharp\net461\EntityComponentSystemCSharp.dll',
                 'bin\Debug\FeatureDetector\net461\FeatureDetector.dll'
)

$nunit = 'nunit.consolerunner\3.10.0\tools\nunit3-console.exe'
$config = Invoke-Expression ".\nuget.exe locals all -list"
foreach($line in $config)
{
    if ($line -match 'global-packages: (?<path>.*)')
    {
        $packagePath = $Matches.path
        $nunitPath = [System.IO.Path]::Combine($packagePath,$nunit)
        if(Test-Path $nunitPath -PathType Leaf)
        {
            $fixtures = [String]::Join(" ", $testFixtures)
            Invoke-Expression "$nunitPath $fixtures"
        }
    }
}