
*The EasyTokens playground demonstrates how interactive scenarios can be built using the APIM TokenStore. 
i.e. store tokens for a application logged in User. 
Once tokens are established for a logged in user, proxy calls to SaaS backends via APIM while attaching the tokens.*

> The playground was built using Static Web Apps, Attached Functions and APIM.

**<font color='red'>DO NOT USE PERSONAL ACCOUNTS</font>**

## Step 1:
Browse to **https://swa.easytokens.in/**

## Step 2:
Login using a test account if you don't have use below\
email: **easytokenstest@M365x713819.onmicrosoft.com**\
password: **ColdFusion1**

## Step 3:
> Tokens will be created and stored under the logged in user

Login using a test account if you don't have use below

graph\
email: **easytokensdemouser@M365x713819.onmicrosoft.com**\
password: **ColdFusion1**

googledrive\
email: **easytokensdemouser@gmail.com**\
password: **ColdFusion1**

dropbox\
email: **easytokensdemouser@gmail.com**\
password: **ColdFusion1**

github\
email: **easytokensdemouser@gmail.com*\
password: **ColdFusion123**

## Step 4: Proxy Attaching Tokens
The playground also has a simple rest client to demonstrate how one could attach tokens to backends while proxying via APIM

See documentation in the site on how to use it.

## Step 5: Get Tokens
Retrieve tokens to inspect. In production this might not be a good practice to give away clients tokens. 
