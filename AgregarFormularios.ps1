# Script para agregar los formularios al archivo .csproj
$csprojPath = "AgenciaViajes.View\AgenciaViajes.View.csproj"

# Leer el contenido del archivo
$content = Get-Content $csprojPath -Raw

# Definir los formularios a agregar
$compileItems = @"
    <Compile Include="frmInicio.cs">
      <SubType>Form</SubType>
    </Compile>
    <Compile Include="frmInicio.Designer.cs">
      <DependentUpon>frmInicio.cs</DependentUpon>
    </Compile>
    <Compile Include="frmClientes.cs">
      <SubType>Form</SubType>
    </Compile>
    <Compile Include="frmClientes.Designer.cs">
      <DependentUpon>frmClientes.cs</DependentUpon>
    </Compile>
    <Compile Include="frmViajes.cs">
      <SubType>Form</SubType>
    </Compile>
    <Compile Include="frmViajes.Designer.cs">
      <DependentUpon>frmViajes.cs</DependentUpon>
    </Compile>
    <Compile Include="frmReservas.cs">
      <SubType>Form</SubType>
    </Compile>
    <Compile Include="frmReservas.Designer.cs">
      <DependentUpon>frmReservas.cs</DependentUpon>
    </Compile>
"@

$resourceItems = @"
    <EmbeddedResource Include="frmInicio.resx">
      <DependentUpon>frmInicio.cs</DependentUpon>
    </EmbeddedResource>
    <EmbeddedResource Include="frmClientes.resx">
      <DependentUpon>frmClientes.cs</DependentUpon>
    </EmbeddedResource>
    <EmbeddedResource Include="frmViajes.resx">
      <DependentUpon>frmViajes.cs</DependentUpon>
    </EmbeddedResource>
    <EmbeddedResource Include="frmReservas.resx">
      <DependentUpon>frmReservas.cs</DependentUpon>
    </EmbeddedResource>
"@

# Buscar e insertar después de Program.cs
$content = $content -replace '(<Compile Include="Program.cs" />)', "`$1`n$compileItems"

# Buscar e insertar antes de Properties\Resources.resx
$content = $content -replace '(<EmbeddedResource Include="Properties\\Resources.resx">)', "$resourceItems`n    `$1"

# Guardar el archivo
$content | Set-Content $csprojPath -Encoding UTF8

Write-Host "Formularios agregados al proyecto exitosamente!" -ForegroundColor Green
Write-Host "Por favor, recarga el proyecto en Visual Studio (Clic derecho en el proyecto > Recargar proyecto)" -ForegroundColor Yellow
