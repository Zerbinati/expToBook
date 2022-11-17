Module modMain
    Public entete As String
    Public positionEPD As String
    Public memPositionEPD As String
    Public memHash As String
    Public exclusion As String

    Sub Main()
        Dim chaine As String, tabChaine() As String, tabTmp() As String, i As Integer, compteur As Integer, tabTampon() As String
        Dim fichierINI As String, fichierEXP As String, moteurEXP As String, lstPositionEPD As String, nbPositionsRetenues As Integer
        Dim position As String, coup As String, horizon As Integer, maxQualite As Integer, pourcentageDispo As Integer, memo As String
        Dim tabPositions(1000000) As String, indexPosition As Integer, offsetPosition As Integer, nbCoupsRetenus As Integer
        Dim qualite As Integer, sommeQualite As Integer, nbCoups As Integer, listeCoupsQualites As String, pourcentage As Integer

        Console.Title = My.Computer.Name

        If My.Computer.FileSystem.GetFileInfo(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) & "Documents\Visual Studio 2013\Projects\expToBook\expToBook\bin\Debug\expToBook.exe").LastWriteTime > My.Computer.FileSystem.GetFileInfo(My.Application.Info.AssemblyName & ".exe").LastWriteTime Then
            MsgBox("Il existe une version plus récente de ce programme !", MsgBoxStyle.Information)
            End
        End If

        fichierINI = My.Computer.Name & ".ini"
        moteurEXP = "D:\JEUX\ARENA CHESS 3.5.1\Engines\Eman\20T Eman 8.20 x64 PCNT.exe"
        fichierEXP = "D:\JEUX\ARENA CHESS 3.5.1\Engines\Eman\Eman.exp"
        If My.Computer.Name = "BOIS" Or My.Computer.Name = "HTPC" Or My.Computer.Name = "TOUR-COURTOISIE" Then
            moteurEXP = "D:\JEUX\ARENA CHESS 3.5.1\Engines\Eman\20T Eman 8.20 x64 BMI2.exe"
            fichierEXP = "D:\JEUX\ARENA CHESS 3.5.1\Engines\Eman\Eman.exp"
        ElseIf My.Computer.Name = "BUREAU" Or My.Computer.Name = "WORKSTATION" Then
            moteurEXP = "E:\JEUX\ARENA CHESS 3.5.1\Engines\Eman\20T Eman 8.20 x64 BMI2.exe"
            fichierEXP = "E:\JEUX\ARENA CHESS 3.5.1\Engines\Eman\Eman.exp"
        End If

        If My.Computer.FileSystem.FileExists(fichierINI) Then
            chaine = My.Computer.FileSystem.ReadAllText(fichierINI)
            If chaine <> "" And InStr(chaine, vbCrLf) > 0 Then
                tabChaine = Split(chaine, vbCrLf)
                For i = 0 To UBound(tabChaine)
                    If tabChaine(i) <> "" And InStr(tabChaine(i), " = ") > 0 Then
                        tabTmp = Split(tabChaine(i), " = ")
                        If tabTmp(0) <> "" And tabTmp(1) <> "" Then
                            If InStr(tabTmp(1), "//") > 0 Then
                                tabTmp(1) = Trim(gauche(tabTmp(1), tabTmp(1).IndexOf("//") - 1))
                            End If
                            Select Case tabTmp(0)
                                Case "moteurEXP"
                                    moteurEXP = Replace(tabTmp(1), """", "")
                                Case "fichierEXP"
                                    fichierEXP = Replace(tabTmp(1), """", "")
                                Case Else

                            End Select
                        End If
                    End If
                Next
            End If
        End If
        My.Computer.FileSystem.WriteAllText(fichierINI, "moteurEXP = " & moteurEXP & vbCrLf _
                                                      & "fichierEXP = " & fichierEXP & vbCrLf, False)

        If Not expV2(fichierEXP) Then
            MsgBox(nomFichier(fichierEXP) & " <> experience format v2 !?", MsgBoxStyle.Exclamation)
            End
        End If

        moteur_court = nomFichier(moteurEXP)

        horizon = 100 'nombre de coups maxi

        position = ""
        If Not My.Computer.FileSystem.FileExists("reprise.ini") Then
            Console.WriteLine("Which position ?")
            Console.WriteLine("(enter an UCI string or leave blank for the default startpos)")
            position = Trim(Console.ReadLine)
        End If
        indexPosition = 0
        tabPositions(0) = position

        offsetPosition = 0
        nbCoupsRetenus = 0
        nbPositionsRetenues = 0

        lstPositionEPD = ""
        memPositionEPD = ""
        memHash = ""
        exclusion = "0000000000000000;"

        Console.Write(vbCrLf & "Loading " & moteur_court & "... ")
        chargerMoteur(moteurEXP, fichierEXP)
        Console.WriteLine("OK" & vbCrLf)
        Console.WriteLine(entete)

        If My.Computer.FileSystem.FileExists("reprise.ini") Then
            chaine = My.Computer.FileSystem.ReadAllText("reprise.ini")
            If chaine <> "" And InStr(chaine, vbCrLf) > 0 Then
                tabChaine = Split(chaine, vbCrLf)
                For i = 0 To UBound(tabChaine)
                    If tabChaine(i) <> "" And InStr(tabChaine(i), " = ") > 0 Then
                        tabTmp = Split(tabChaine(i), " = ")
                        If tabTmp(0) <> "" And tabTmp(1) <> "" Then
                            If InStr(tabTmp(1), "//") > 0 Then
                                tabTmp(1) = Trim(gauche(tabTmp(1), tabTmp(1).IndexOf("//") - 1))
                            End If
                            Select Case tabTmp(0)
                                Case "indexPosition"
                                    indexPosition = CInt(Trim(tabTmp(1)))
                                    tabTampon = Split(My.Computer.FileSystem.ReadAllText("tabPositions.txt"), vbCrLf)
                                    ReDim tabPositions(1000000)
                                    Array.Copy(tabTampon, 0, tabPositions, indexPosition, tabTampon.Length)

                                Case "offsetPosition"
                                    offsetPosition = CInt(Trim(tabTmp(1)))

                                Case "nbCoupsRetenus"
                                    nbCoupsRetenus = CInt(Trim(tabTmp(1)))

                                Case "nbPositionsRetenues"
                                    nbPositionsRetenues = CInt(Trim(tabTmp(1)))

                                Case "lstPositionEPD"
                                    lstPositionEPD = Trim(tabTmp(1))

                                Case "memPositionEPD"
                                    memPositionEPD = Trim(tabTmp(1))

                                Case "memHash"
                                    memHash = Trim(tabTmp(1))

                                Case "exclusion"
                                    exclusion = Trim(tabTmp(1))

                                Case Else

                            End Select
                        End If
                    End If
                Next
            End If
        Else
            If My.Computer.FileSystem.FileExists("main.bin") Then
                My.Computer.FileSystem.DeleteFile("main.bin")
            End If
        End If

        If My.Computer.FileSystem.FileExists("temp.bin") Then
            My.Computer.FileSystem.DeleteFile("temp.bin")
        End If

        Do
            position = tabPositions(indexPosition)
            chaine = expListe(position)
            If chaine <> "" And InStr(lstPositionEPD, gauche(positionEPD, positionEPD.IndexOf(" ") + 2), CompareMethod.Text) = 0 Then
                tabChaine = Split(chaine, vbCrLf)

                'on récupère les 3 meilleurs coups et leur qualité

                listeCoupsQualites = ""
                sommeQualite = 0
                nbCoups = 0
                maxQualite = -1000
                For i = 0 To UBound(tabChaine)
                    If tabChaine(i) <> "" And InStr(tabChaine(i), "mate", CompareMethod.Text) = 0 Then
                        tabTmp = Split(Replace(tabChaine(i), ":", ","), ",")

                        coup = Trim(tabTmp(1))
                        chaine = Trim(position & " " & coup)

                        If nbCoups < 4 Then
                            qualite = CInt(Trim(tabTmp(9)))
                            compteur = CInt(Trim(tabTmp(7)))
                            If qualite > maxQualite Then
                                maxQualite = qualite
                            End If
                            If 0 < qualite _
                            And ((position = "") Or (position <> "" And maxQualite < 1.2 * qualite)) _
                            And Not (1 <= nbCoups And compteur = 1) Then
                                sommeQualite = sommeQualite + qualite
                                listeCoupsQualites = listeCoupsQualites & coup & ";" & qualite & vbCrLf
                                nbCoupsRetenus = nbCoupsRetenus + 1
                            End If
                            nbCoups = nbCoups + 1
                        End If

                        If offsetPosition < 1000000 Then
                            offsetPosition = offsetPosition + 1
                            If offsetPosition > UBound(tabPositions) Then
                                ReDim Preserve tabPositions(offsetPosition * 1.1)
                            End If
                            tabPositions(offsetPosition) = chaine
                        End If
                    End If
                Next

                'on calcule les pourcentages
                If InStr(listeCoupsQualites, vbCrLf) > 0 Then
                    nbPositionsRetenues = nbPositionsRetenues + 1
                    tabChaine = Split(listeCoupsQualites, vbCrLf)
                    nbCoups = 0
                    chaine = ""
                    pourcentageDispo = 100
                    For i = 0 To UBound(tabChaine)
                        If tabChaine(i) <> "" And InStr(tabChaine(i), "mate", CompareMethod.Text) = 0 Then
                            tabTmp = Split(tabChaine(i), ";")

                            coup = Trim(tabTmp(0))
                            qualite = CInt(Trim(tabTmp(1)))
                            pourcentage = qualite * 100 / sommeQualite

                            nbCoups = nbCoups + 1
                            If pourcentageDispo < pourcentage Then
                                pourcentage = pourcentageDispo
                            Else
                                pourcentageDispo = pourcentageDispo - pourcentage
                            End If
                            chaine = chaine & coup & " @ " & pourcentage & "% (Q" & qualite & "), "

                            entreePolyglot(positionEPD, coup, pourcentage)

                        End If
                    Next
                End If

                If indexPosition Mod 20 = 0 Then
                    Console.Clear()
                    If position = "" Then
                        Console.WriteLine("position   : " & positionEPD)
                    Else
                        Console.WriteLine("position   : " & position)
                    End If
                    If chaine = "" Then
                        Console.WriteLine("book moves : " & vbCrLf)
                    Else
                        Console.WriteLine("book moves : " & gauche(chaine, Len(chaine) - 2) & vbCrLf)
                    End If
                    Console.WriteLine("|----------------------|----------------------|")
                    Console.WriteLine("|   experience data    |     opening book     |")
                    Console.WriteLine("|----------------------|----------------------|")
                    Console.WriteLine("| " & formaterChaine(Trim(Format(offsetPosition, "00 000 000 moves"))) & "     | " & formaterChaine(Trim(Format(nbCoupsRetenus, "00 000 000 moves"))) & "     |")
                    Console.WriteLine("| " & formaterChaine(Trim(Format(indexPosition, "00 000 000 positions"))) & " | " & formaterChaine(Trim(Format(nbPositionsRetenues, "00 000 000 positions"))) & " |")
                    Console.WriteLine("| " & Trim(Format(offsetPosition / nbPositionsRetenues, "average 00 moves/pos")) & " | " & Trim(Format(nbCoupsRetenus / nbPositionsRetenues, "average 00 moves/pos")) & " |")
                    Console.WriteLine("|----------------------|----------------------|")
                End If

                lstPositionEPD = lstPositionEPD & gauche(positionEPD, positionEPD.IndexOf(" ") + 2) & ";"

                If nbPositionsRetenues Mod 1000 = 0 Then
                    trier("main.bin", "temp.bin")

                    My.Computer.FileSystem.WriteAllText("reprise.ini", "indexPosition = " & Format(indexPosition + 1) & vbCrLf _
                                                                     & "offsetPosition = " & offsetPosition & vbCrLf _
                                                                     & "nbCoupsRetenus = " & nbCoupsRetenus & vbCrLf _
                                                                     & "nbPositionsRetenues = " & nbPositionsRetenues & vbCrLf _
                                                                     & "lstPositionEPD = " & lstPositionEPD & vbCrLf _
                                                                     & "memPositionEPD = " & memPositionEPD & vbCrLf _
                                                                     & "memHash = " & memHash & vbCrLf _
                                                                     & "exclusion = " & exclusion & vbCrLf, False)
                    memo = Console.Title
                    Console.Title = "tabPositions.txt..."
                    chaine = ""
                    For i = indexPosition + 1 To offsetPosition
                        chaine = chaine & tabPositions(i) & vbCrLf
                    Next
                    My.Computer.FileSystem.WriteAllText("tabPositions.txt", chaine, False)
                    Console.Title = memo
                End If
            End If

            indexPosition = indexPosition + 1
        Loop While tabPositions(indexPosition) <> "" And nbCaracteres(tabPositions(indexPosition), " ") < (horizon - 1) And offsetPosition < 1000000

        Console.Clear()
        Console.WriteLine("|----------------------|----------------------|")
        Console.WriteLine("|   experience data    |     opening book     |")
        Console.WriteLine("|----------------------|----------------------|")
        Console.WriteLine("| " & formaterChaine(Trim(Format(offsetPosition, "00 000 000 moves"))) & "     | " & formaterChaine(Trim(Format(nbCoupsRetenus, "00 000 000 moves"))) & "     |")
        Console.WriteLine("| " & formaterChaine(Trim(Format(indexPosition, "00 000 000 positions"))) & " | " & formaterChaine(Trim(Format(nbPositionsRetenues, "00 000 000 positions"))) & " |")
        Console.WriteLine("| " & Trim(Format(offsetPosition / nbPositionsRetenues, "average 00 moves/pos")) & " | " & Trim(Format(nbCoupsRetenus / nbPositionsRetenues, "average 00 moves/pos")) & " |")
        Console.WriteLine("|----------------------|----------------------|")

        trier("main.bin", "temp.bin")
        If My.Computer.FileSystem.FileExists("reprise.ini") Then
            My.Computer.FileSystem.DeleteFile("reprise.ini")
        End If
        If My.Computer.FileSystem.FileExists("tabPositions.txt") Then
            My.Computer.FileSystem.DeleteFile("tabPositions.txt")
        End If

        tabPositions = Nothing

        dechargerMoteur()

        Console.WriteLine("Press ENTER to close the window.")
        Console.ReadLine()
    End Sub

    Public Sub entreePolyglot(positionEPD As String, coup As String, pourcentage As Integer)
        Dim tabLivre(15) As Byte, hash As String, offset As Integer, i As Integer, chaine As String
        Dim roqueDispo As String, tabChaine() As String

        'position
        If memPositionEPD = positionEPD Then
            hash = memHash
        Else
            memPositionEPD = positionEPD
            hash = calculHash(positionEPD)
            hash = UCase(gauche(hash, 16))
            memHash = hash
        End If

        'roque expList <> roque polyglot
        tabChaine = Split(positionEPD, " ")
        roqueDispo = tabChaine(2)
        If roqueDispo <> "-" Then
            If coup = "e1g1" And InStr(positionEPD, " w ") > 0 And InStr(roqueDispo, "K") > 0 Then
                coup = "e1h1"
            ElseIf coup = "e8g8" And InStr(positionEPD, " b ") > 0 And InStr(roqueDispo, "k") > 0 Then
                coup = "e8h8"
            ElseIf coup = "e1c1" And InStr(positionEPD, " w ") > 0 And InStr(roqueDispo, "Q") > 0 Then
                coup = "e1a1"
            ElseIf coup = "e8c8" And InStr(positionEPD, " b ") > 0 And InStr(roqueDispo, "q") > 0 Then
                coup = "e8a8"
            End If
        End If

        If InStr(exclusion, hash & ":" & coup) = 0 Then
            exclusion = exclusion & hash & ":" & coup & ";"

            offset = 0
            For i = 0 To 14 Step 2
                tabLivre(offset) = Byte.Parse(hash.Substring(i, 2), Globalization.NumberStyles.HexNumber)
                offset = offset + 1
            Next

            'coups
            chaine = coupBinaire(coup)
            chaine = binHex(chaine)
            If Len(chaine) = 1 Then
                tabLivre(8) = 0
                tabLivre(9) = Byte.Parse(chaine.Substring(0, 1), Globalization.NumberStyles.HexNumber)
            ElseIf Len(chaine) = 2 Then
                tabLivre(8) = 0
                tabLivre(9) = Byte.Parse(chaine.Substring(0, 2), Globalization.NumberStyles.HexNumber)
            ElseIf Len(chaine) = 3 Then
                tabLivre(8) = Byte.Parse(chaine.Substring(0, 1), Globalization.NumberStyles.HexNumber)
                tabLivre(9) = Byte.Parse(chaine.Substring(1, 2), Globalization.NumberStyles.HexNumber)
            ElseIf Len(chaine) = 4 Then
                tabLivre(8) = Byte.Parse(chaine.Substring(0, 2), Globalization.NumberStyles.HexNumber)
                tabLivre(9) = Byte.Parse(chaine.Substring(2, 2), Globalization.NumberStyles.HexNumber)
            End If

            'poids
            tabLivre(10) = 0
            tabLivre(11) = pourcentage

            My.Computer.FileSystem.WriteAllBytes("temp.bin", tabLivre, True)
        Else
            MsgBox("doublon")
        End If
    End Sub

    Private Function formaterChaine(chaine As String) As String
        If gauche(chaine, 9) = "00 000 00" Then
            chaine = Replace(chaine, "00 000 00", StrDup(9, "."), , 1)
        End If
        If gauche(chaine, 8) = "00 000 0" Then
            chaine = Replace(chaine, "00 000 0", StrDup(8, "."), , 1)
        End If
        If gauche(chaine, 7) = "00 000 " Then
            chaine = Replace(chaine, "00 000 ", StrDup(7, "."), , 1)
        End If
        If gauche(chaine, 6) = "00 000" Then
            chaine = Replace(chaine, "00 000", StrDup(6, "."), , 1)
        End If
        If gauche(chaine, 5) = "00 00" Then
            chaine = Replace(chaine, "00 00", StrDup(5, "."), , 1)
        End If
        If gauche(chaine, 4) = "00 0" Then
            chaine = Replace(chaine, "00 0", StrDup(4, "."), , 1)
        End If
        If gauche(chaine, 3) = "00 " Then
            chaine = Replace(chaine, "00 ", StrDup(3, "."), , 1)
        End If
        If gauche(chaine, 2) = "0" Then
            chaine = Replace(chaine, "0", ".", , 1)
        End If
        Return chaine
    End Function

End Module
