<#
  LMS API smoke test.
  1. Start the API:  dotnet run --project src/LMS.API --urls "http://localhost:5000" --environment Development
  2. In another PowerShell window:  .\smoke-test.ps1
  Creates fresh test data every run (random suffix) so it never hits "duplicate" errors.
#>
param(
    [string]$BaseUrl = "http://localhost:5000",
    [string]$AdminEmail = "admin@library.com",
    [string]$AdminPassword = "Admin@123"
)

$ErrorActionPreference = "Stop"
$suffix  = Get-Random -Minimum 10000 -Maximum 99999
$script:Headers = @{}
$script:Results = @()

function Call {
    param([string]$Method, [string]$Path, $Body = $null)
    $p = @{ Uri = "$BaseUrl$Path"; Method = $Method; ContentType = "application/json"; Headers = $script:Headers }
    if ($null -ne $Body) { $p.Body = ($Body | ConvertTo-Json -Depth 6) }
    try {
        return Invoke-RestMethod @p
    }
    catch {
        Write-Host ""
        Write-Host "FAILED: $Method $Path" -ForegroundColor Red
        if ($_.ErrorDetails.Message) { Write-Host $_.ErrorDetails.Message -ForegroundColor Red }
        else { Write-Host $_.Exception.Message -ForegroundColor Red }
        Write-Host "Check the API console window for the [ERR] block and paste it to Claude." -ForegroundColor Yellow
        throw
    }
}

function Check {
    param([string]$Name, $Actual, $Expected)
    $ok = ("$Actual" -eq "$Expected")
    $script:Results += [pscustomobject]@{ Check = $Name; Expected = $Expected; Actual = $Actual; Result = $(if ($ok) { "PASS" } else { "FAIL" }) }
    $color = if ($ok) { "Green" } else { "Red" }
    Write-Host ("  [{0}] {1} (expected {2}, got {3})" -f $(if ($ok) { "PASS" } else { "FAIL" }), $Name, $Expected, $Actual) -ForegroundColor $color
}

function Step([string]$text) { Write-Host ""; Write-Host "== $text" -ForegroundColor Cyan }

Step "1. Login as admin"
$login = Call POST "/api/Auth/login" @{ Email = $AdminEmail; Password = $AdminPassword; RememberMe = $false }
$script:Headers = @{ Authorization = "Bearer $($login.AccessToken)" }
Check "login returns token" ([bool]$login.AccessToken) "True"

Step "2. Category, publisher, author"
$cat = Call POST "/api/Catalog/categories" @{ Name = "SmokeCat$suffix"; Description = "smoke"; DisplayOrder = 1 }
$pub = Call POST "/api/Catalog/publishers" @{ Name = "SmokePub$suffix" }
$aut = Call POST "/api/Catalog/authors" @{ FirstName = "Smoke"; LastName = "Author$suffix" }
Check "category created" ([bool]$cat.Id) "True"
Check "publisher created" ([bool]$pub.Id) "True"
Check "author created" ([bool]$aut.Id) "True"

Step "3. Book (with author) and one copy"
$isbn = "978" + (Get-Random -Minimum 1000000000 -Maximum 1999999999)
$book = Call POST "/api/Catalog/books" @{
    Title = "Smoke Book $suffix"; ISBN = $isbn; PublicationYear = 2020; Language = "English"; Pages = 100
    CategoryId = $cat.Id; PublisherId = $pub.Id; AuthorIds = @($aut.Id)
}
Check "book has category name" $book.CategoryName "SmokeCat$suffix"
Check "book has 1 author" (@($book.Authors).Count) "1"
$copy = Call POST "/api/Catalog/copies" @{
    BookId = $book.Id; CopyNumber = "C-$suffix"; Location = "Shelf A1"; Barcode = "BC$suffix"
    AcquisitionDate = (Get-Date).ToUniversalTime().ToString("o"); Cost = 100; Condition = "Good"; Status = 1
}
Check "copy created" ([bool]$copy.Id) "True"

Step "4. Member (register user, then membership)"
$email = "smoke$suffix@example.com"
$reg = Call POST "/api/Auth/register" @{
    Email = $email; Password = "Test@1234"; ConfirmPassword = "Test@1234"
    FirstName = "Smoke"; LastName = "User$suffix"; PhoneNumber = "9999999999"
}
$types = Call GET "/api/Member/types"
$typeId = @($types.Items)[0].Id
$member = Call POST "/api/Member" @{
    UserId = $reg.User.Id; FirstName = "Smoke"; LastName = "User$suffix"; Email = $email
    DateOfBirth = "2000-01-01T00:00:00Z"; MemberTypeId = $typeId
    ExpiryDate = (Get-Date).AddYears(1).ToUniversalTime().ToString("o")
}
Check "member created" ([bool]$member.Id) "True"

Step "5. Issue the loan"
$loan = Call POST "/api/Circulation/loans/issue" @{ MemberId = $member.Id; BookCopyId = $copy.Id; Notes = "smoke" }
Check "loan created" ([bool]$loan.Id) "True"
Check "loan status Active (1)" ([int]$loan.Status) "1"
$copyAfter = Call GET "/api/Catalog/copies/$($copy.Id)"
Check "copy status Issued (2) after issue" ([int]$copyAfter.Status) "2"
$bookAfter = Call GET "/api/Catalog/books/$($book.Id)"
Check "book AvailableCopies is 0 after issue" ([int]$bookAfter.AvailableCopies) "0"

Step "6. Return the loan"
$ret = Call POST "/api/Circulation/loans/return" @{ LoanId = $loan.Id; Condition = "Good"; Notes = "smoke return" }
Check "loan status Returned (2)" ([int]$ret.Status) "2"
$copyBack = Call GET "/api/Catalog/copies/$($copy.Id)"
Check "copy status Available (1) after return" ([int]$copyBack.Status) "1"
$bookBack = Call GET "/api/Catalog/books/$($book.Id)"
Check "book AvailableCopies is 1 after return" ([int]$bookBack.AvailableCopies) "1"

Step "Summary"
$script:Results | Format-Table -AutoSize
$failed = @($script:Results | Where-Object { $_.Result -eq "FAIL" }).Count
if ($failed -eq 0) { Write-Host "ALL CHECKS PASSED" -ForegroundColor Green }
else { Write-Host "$failed CHECK(S) FAILED - paste this summary table to Claude" -ForegroundColor Red }
