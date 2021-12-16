;#DropDB XafIPFSEasyTest

#Application XafIPFSWin
#Application XafIPFSBlazor

*FillForm
 User Name = Admin
*Action Log In

#IfDef XafIPFSWin

*Action About...
 
*Action OK

#EndIf

#IfDef XafIPFSBlazor
*Action Navigation(My Details)
#EndIf
