# Code Signing – HTREASURE LTD

Unsigned installers make Windows SmartScreen show **"Unknown publisher"**. Signing needs a code-signing certificate issued to **HTREASURE LTD** after the issuer has verified the company. Only HTREASURE LTD can apply for one. The build pipeline is ready for either route below. A self-signed certificate is never used: it would still show "Unknown publisher".

## Route B (recommended): Azure Artifact Signing

Microsoft's managed signing service (formerly *Trusted Signing*). Microsoft lists organisations in the United Kingdom as eligible for public-trust certificates. There's no hardware token, the private key never leaves Microsoft, and it works in GitHub Actions.

1. Create an Azure subscription for HTREASURE LTD, then an **Artifact Signing account** in the Azure portal. Note the region **endpoint**, for example `https://weu.codesigning.azure.net/`.
2. Complete **identity validation** for the organisation (company number, registered address, a verifiable contact). Approval can take several days.
3. Create a **Public Trust certificate profile**, and give the signing identity the *Artifact Signing Certificate Profile Signer* role.
4. Install the Windows SDK (signtool) and Microsoft's signing client, which provides `Azure.CodeSigning.Dlib.dll`.
5. Build a signed release:

```bash
az login
```

```bash
powershell -Command "$env:HT_ARTIFACT_SIGNING_ENDPOINT='https://weu.codesigning.azure.net/'; $env:HT_ARTIFACT_SIGNING_ACCOUNT='<account>'; $env:HT_ARTIFACT_SIGNING_PROFILE='<profile>'; $env:HT_ARTIFACT_SIGNING_DLIB='<path>\Azure.CodeSigning.Dlib.dll'; .\scripts\build-release.ps1"
```

In GitHub Actions, sign in with a service principal by setting `AZURE_CLIENT_ID`, `AZURE_TENANT_ID` and `AZURE_CLIENT_SECRET` as secrets.

If identity validation can't be completed, use Route A.

## Route A: OV or EV certificate from a commercial certificate authority

Buy an Organisation Validation (OV) or Extended Validation (EV) code-signing certificate for HTREASURE LTD from an authority such as DigiCert, Sectigo or GlobalSign. Since 2023 the private key must be kept on a hardware token or HSM. Install the token's driver so the certificate appears in the Windows certificate store, then:

```bash
powershell -Command "$env:HT_SIGN_THUMBPRINT='<certificate thumbprint>'; $env:HT_TIMESTAMP_URL='<the authority timestamp URL>'; .\scripts\build-release.ps1"
```

Certificates on hardware tokens can't be used from GitHub-hosted runners. Sign on the PC that holds the token.

## What gets signed

1. Every `.exe` and `.dll` of all four application builds, before packaging, so the installers and portable ZIPs contain signed files.
2. Both Setup programs and their uninstallers (Inno Setup `SignTool` with `SignedUninstaller`).

All signatures use SHA-256 and an RFC 3161 timestamp, so they stay valid after the certificate expires. The script then verifies every signature and stops on any that isn't valid; Inno Setup also refuses to finish if a signing call produced no signature. `release-manifest.json` records `"signed": true`.

## Checking a signed release

```bash
signtool verify /pa /v HT-ISO-Ripper-Setup-1.0.0.exe
```

Or: right-click the file → Properties → Digital Signatures → *HTREASURE LTD*.

SmartScreen reputation builds over time, even for signed files. Organisation certificates from Artifact Signing (and EV certificates) usually establish it fastest.
