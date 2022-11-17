Imports VB = Microsoft.VisualBasic

Module modFonctions
    Public processus As System.Diagnostics.Process
    Public entree As System.IO.StreamWriter
    Public sortie As System.IO.StreamReader
    Public moteur_court As String

    Public Function binHex(suite As String) As String
        Dim i As Integer, cumul As Integer

        cumul = 0
        For i = 1 To Len(suite)
            cumul = cumul + CInt(gauche(droite(suite, i), 1)) * 2 ^ (i - 1)
        Next
        binHex = Hex(cumul)
    End Function

    Public Function calculHash(epd As String) As String
        Dim processus As New System.Diagnostics.Process()

        processus.StartInfo.RedirectStandardOutput = True
        processus.StartInfo.UseShellExecute = False
        processus.StartInfo.CreateNoWindow = True
        processus.StartInfo.FileName = "pg_key.exe"
        processus.StartInfo.Arguments = """" & epd & """"
        processus.Start()
        calculHash = processus.StandardOutput.ReadToEnd
        processus.Close()
        processus = Nothing
    End Function

    Public Sub chargerMoteur(chemin As String, fichierEXP As String)
        Dim chaine As String

chargement_moteur:
        Try
            processus = New System.Diagnostics.Process()

            processus.StartInfo.RedirectStandardOutput = True
            processus.StartInfo.UseShellExecute = False
            processus.StartInfo.RedirectStandardInput = True
            processus.StartInfo.CreateNoWindow = True
            processus.StartInfo.WorkingDirectory = My.Application.Info.DirectoryPath
            processus.StartInfo.FileName = chemin
            processus.Start()
            processus.PriorityClass = 64 '64 (idle), 16384 (below normal), 32 (normal), 32768 (above normal), 128 (high), 256 (realtime)

            entree = processus.StandardInput
            sortie = processus.StandardOutput

            entree.WriteLine("uci")
            chaine = ""
            While InStr(chaine, "uciok") = 0
                If processus.HasExited Then
                    entree.Close()
                    sortie.Close()
                    processus.Close()
                    GoTo chargement_moteur
                End If
                chaine = sortie.ReadLine
                Threading.Thread.Sleep(10)
            End While

            entree.WriteLine("setoption name threads value 1")
            entree.WriteLine("setoption name Experience File value " & fichierEXP)

            entete = ""
            While entete = ""
                If processus.HasExited Then
                    entree.Close()
                    sortie.Close()
                    processus.Close()
                    GoTo chargement_moteur
                End If
                chaine = sortie.ReadLine
                If InStr(chaine, "info", CompareMethod.Text) > 0 _
                And InStr(chaine, "string", CompareMethod.Text) > 0 _
                And (InStr(chaine, "collision", CompareMethod.Text) > 0 Or InStr(chaine, "duplicate", CompareMethod.Text) > 0) Then
                    entete = Replace(chaine, fichierEXP, nomFichier(fichierEXP)) & vbCrLf
                End If
                Threading.Thread.Sleep(10)
            End While

            entree.WriteLine("isready")
            chaine = ""
            While InStr(chaine, "readyok") = 0
                If processus.HasExited Then
                    entree.Close()
                    sortie.Close()
                    processus.Close()
                    GoTo chargement_moteur
                End If
                chaine = sortie.ReadLine
                Threading.Thread.Sleep(10)
            End While
        Catch ex As Exception
            If processus.HasExited Then
                entree.Close()
                sortie.Close()
                processus.Close()
                GoTo chargement_moteur
            End If
        End Try

    End Sub

    Public Function coupBinaire(coup As String) As String
        coupBinaire = "0 000 "
        If InStr(coup, "=") > 0 Then
            Select Case droite(coup, 1)
                Case "C"
                    coupBinaire = "0 001 "
                Case "F"
                    coupBinaire = "0 010 "
                Case "T"
                    coupBinaire = "0 011 "
                Case "D"
                    coupBinaire = "0 100 "
            End Select
        End If


        'ligne de départ
        If coup.Substring(1, 1) = "1" Then
            coupBinaire = coupBinaire & "000" & " "
        ElseIf coup.Substring(1, 1) = "2" Then
            coupBinaire = coupBinaire & "001" & " "
        ElseIf coup.Substring(1, 1) = "3" Then
            coupBinaire = coupBinaire & "010" & " "
        ElseIf coup.Substring(1, 1) = "4" Then
            coupBinaire = coupBinaire & "011" & " "
        ElseIf coup.Substring(1, 1) = "5" Then
            coupBinaire = coupBinaire & "100" & " "
        ElseIf coup.Substring(1, 1) = "6" Then
            coupBinaire = coupBinaire & "101" & " "
        ElseIf coup.Substring(1, 1) = "7" Then
            coupBinaire = coupBinaire & "110" & " "
        ElseIf coup.Substring(1, 1) = "8" Then
            coupBinaire = coupBinaire & "111" & " "
        End If

        'colonne de départ
        If coup.Substring(0, 1) = "a" Then
            coupBinaire = coupBinaire & "000" & " "
        ElseIf coup.Substring(0, 1) = "b" Then
            coupBinaire = coupBinaire & "001" & " "
        ElseIf coup.Substring(0, 1) = "c" Then
            coupBinaire = coupBinaire & "010" & " "
        ElseIf coup.Substring(0, 1) = "d" Then
            coupBinaire = coupBinaire & "011" & " "
        ElseIf coup.Substring(0, 1) = "e" Then
            coupBinaire = coupBinaire & "100" & " "
        ElseIf coup.Substring(0, 1) = "f" Then
            coupBinaire = coupBinaire & "101" & " "
        ElseIf coup.Substring(0, 1) = "g" Then
            coupBinaire = coupBinaire & "110" & " "
        ElseIf coup.Substring(0, 1) = "h" Then
            coupBinaire = coupBinaire & "111" & " "
        End If

        'ligne d'arrivée
        If coup.Substring(3, 1) = "1" Then
            coupBinaire = coupBinaire & "000" & " "
        ElseIf coup.Substring(3, 1) = "2" Then
            coupBinaire = coupBinaire & "001" & " "
        ElseIf coup.Substring(3, 1) = "3" Then
            coupBinaire = coupBinaire & "010" & " "
        ElseIf coup.Substring(3, 1) = "4" Then
            coupBinaire = coupBinaire & "011" & " "
        ElseIf coup.Substring(3, 1) = "5" Then
            coupBinaire = coupBinaire & "100" & " "
        ElseIf coup.Substring(3, 1) = "6" Then
            coupBinaire = coupBinaire & "101" & " "
        ElseIf coup.Substring(3, 1) = "7" Then
            coupBinaire = coupBinaire & "110" & " "
        ElseIf coup.Substring(3, 1) = "8" Then
            coupBinaire = coupBinaire & "111" & " "
        End If

        'colonne d'arrivée
        If coup.Substring(2, 1) = "a" Then
            coupBinaire = coupBinaire & "000"
        ElseIf coup.Substring(2, 1) = "b" Then
            coupBinaire = coupBinaire & "001"
        ElseIf coup.Substring(2, 1) = "c" Then
            coupBinaire = coupBinaire & "010"
        ElseIf coup.Substring(2, 1) = "d" Then
            coupBinaire = coupBinaire & "011"
        ElseIf coup.Substring(2, 1) = "e" Then
            coupBinaire = coupBinaire & "100"
        ElseIf coup.Substring(2, 1) = "f" Then
            coupBinaire = coupBinaire & "101"
        ElseIf coup.Substring(2, 1) = "g" Then
            coupBinaire = coupBinaire & "110"
        ElseIf coup.Substring(2, 1) = "h" Then
            coupBinaire = coupBinaire & "111"
        End If

        coupBinaire = Replace(coupBinaire, " ", "")
    End Function

    Public Sub dechargerMoteur()
        entree.Close()
        sortie.Close()
        processus.Close()

        entree = Nothing
        sortie = Nothing
        processus = Nothing
    End Sub

    Public Function droite(texte As String, longueur As Integer) As String
        If longueur > 0 Then
            Return VB.Right(texte, longueur)
        Else
            Return ""
        End If
    End Function

    Public Function expListe(position As String) As String
        Dim chaine As String, ligne As String

        If position = "" Then
            entree.WriteLine("position startpos")
        ElseIf InStr(position, "/", CompareMethod.Text) > 0 _
          And (InStr(position, " w ", CompareMethod.Text) > 0 Or InStr(position, " b ", CompareMethod.Text) > 0) Then
            entree.WriteLine("position fen " & position)
        Else
            entree.WriteLine("position startpos moves " & position)
        End If
        entree.WriteLine("expex")

        entree.WriteLine("isready")

        chaine = ""
        ligne = ""
        While InStr(ligne, "readyok") = 0
            ligne = sortie.ReadLine
            If InStr(ligne, "Fen: ", CompareMethod.Text) > 0 Then
                positionEPD = Trim(Replace(ligne, "Fen: ", ""))
            ElseIf InStr(ligne, "quality:") > 0 Then
                chaine = chaine & ligne & vbCrLf
            End If
        End While

        Return chaine
    End Function

    Public Function expV2(cheminEXP As String) As Boolean
        Dim lecture As IO.FileStream, tampon As Long, tabTampon() As Byte

        lecture = New IO.FileStream(cheminEXP, IO.FileMode.Open)

        'SugaR Experience version 2
        '0123456789abcdef0123456789
        tampon = 26
        ReDim tabTampon(tampon - 1)
        lecture.Read(tabTampon, 0, tampon)
        lecture.Close()

        If System.Text.Encoding.UTF8.GetString(tabTampon) <> "SugaR Experience version 2" Then
            Return False
        Else
            Return True
        End If

    End Function

    Public Function gauche(texte As String, longueur As Integer) As String
        If longueur > 0 Then
            Return VB.Left(texte, longueur)
        Else
            Return ""
        End If
    End Function

    Public Function nbCaracteres(ByVal chaine As String, ByVal critere As String) As Integer
        Return Len(chaine) - Len(Replace(chaine, critere, ""))
    End Function

    Public Function nettoyage(coup As String, epd As String) As String
        Dim promotion As String

        If coup = "-" Or coup = "" Then
            Return ""
        End If

        promotion = ""
        If InStr(coup, "=") > 0 Then
            'comme ça en évite le # dans g2-g1=D#
            promotion = coup.Substring(coup.IndexOf("=") + 1, 1)
        End If

        coup = Replace(coup, "C", "")
        coup = Replace(coup, "D", "")
        coup = Replace(coup, "F", "")
        coup = Replace(coup, "R", "")
        coup = Replace(coup, "T", "")
        coup = Replace(coup, "x", "")
        coup = Replace(coup, "-", "")
        coup = Replace(coup, "+", "")
        coup = Replace(coup, "#", "")

        'roque
        If coup = "00" And InStr(epd, " w ") > 0 Then
            coup = "e1h1"
        ElseIf coup = "00" And InStr(epd, " b ") > 0 Then
            coup = "e8h8"
        ElseIf coup = "000" And InStr(epd, " w ") > 0 Then
            coup = "e1a1"
        ElseIf coup = "000" And InStr(epd, " b ") > 0 Then
            coup = "e8a8"
        End If

        'promotion (en travaux)
        If promotion <> "" Then
            coup = coup & promotion
        End If

        Return coup
    End Function

    Public Function nomFichier(chemin As String) As String
        Return My.Computer.FileSystem.GetName(chemin)
    End Function

    Public Sub trier(cheminBIN1 As String, cheminBIN2 As String)
        Dim tabBIN(0) As Byte, tabTMP() As Byte, tampon(15) As Byte, i As Integer, j As Integer, k As Integer
        Dim memo As String, longueur As Integer, indexBIN As Integer

        memo = Console.Title

        longueur = 0
        indexBIN = 0

        If My.Computer.FileSystem.FileExists(cheminBIN1) Then
            longueur = FileLen(cheminBIN1)
            tabTMP = My.Computer.FileSystem.ReadAllBytes(cheminBIN1)
            tabBIN = tabTMP
            indexBIN = tabBIN.Length
        End If

        If My.Computer.FileSystem.FileExists(cheminBIN2) Then
            longueur = longueur + FileLen(cheminBIN2)
            tabTMP = My.Computer.FileSystem.ReadAllBytes(cheminBIN2)
            ReDim Preserve tabBIN(longueur - 1)
            Array.Copy(tabTMP, 0, tabBIN, indexBIN, UBound(tabTMP) + 1)
        End If

        For i = 0 To UBound(tabBIN) Step 16
            If i Mod 10000 = 0 Then
                Console.Title = "trie #1 @ " & Format(i / UBound(tabBIN), "0.00%") & " (" & i & " / " & UBound(tabBIN) & ")"
            End If
            For j = 0 To UBound(tabBIN) Step 16
                If tabBIN(i) < tabBIN(j) Then
                    For k = 0 To UBound(tampon)
                        tampon(k) = tabBIN(j + k)
                        tabBIN(j + k) = tabBIN(i + k)
                        tabBIN(i + k) = tampon(k)
                    Next
                End If
            Next
        Next

        For i = 0 To UBound(tabBIN) Step 16
            If i Mod 10000 = 0 Then
                Console.Title = "trie #2 @ " & Format(i / UBound(tabBIN), "0.00%") & " (" & i & " / " & UBound(tabBIN) & ")"
            End If
            For j = 0 To UBound(tabBIN) Step 16
                If tabBIN(i) = tabBIN(j) And tabBIN(i + 1) < tabBIN(j + 1) Then
                    For k = 0 To UBound(tampon)
                        tampon(k) = tabBIN(j + k)
                        tabBIN(j + k) = tabBIN(i + k)
                        tabBIN(i + k) = tampon(k)
                    Next
                End If
            Next
        Next

        For i = 0 To UBound(tabBIN) Step 16
            If i Mod 10000 = 0 Then
                Console.Title = "trie #3 @ " & Format(i / UBound(tabBIN), "0.00%") & " (" & i & " / " & UBound(tabBIN) & ")"
            End If
            For j = 0 To UBound(tabBIN) Step 16
                If tabBIN(i) = tabBIN(j) And tabBIN(i + 1) = tabBIN(j + 1) And tabBIN(i + 2) < tabBIN(j + 2) Then
                    For k = 0 To UBound(tampon)
                        tampon(k) = tabBIN(j + k)
                        tabBIN(j + k) = tabBIN(i + k)
                        tabBIN(i + k) = tampon(k)
                    Next
                End If
            Next
        Next

        For i = 0 To UBound(tabBIN) Step 16
            If i Mod 10000 = 0 Then
                Console.Title = "trie #4 @ " & Format(i / UBound(tabBIN), "0.00%") & " (" & i & " / " & UBound(tabBIN) & ")"
            End If
            For j = 0 To UBound(tabBIN) Step 16
                If tabBIN(i) = tabBIN(j) And tabBIN(i + 1) = tabBIN(j + 1) And tabBIN(i + 2) = tabBIN(j + 2) And tabBIN(i + 3) < tabBIN(j + 3) Then
                    For k = 0 To UBound(tampon)
                        tampon(k) = tabBIN(j + k)
                        tabBIN(j + k) = tabBIN(i + k)
                        tabBIN(i + k) = tampon(k)
                    Next
                End If
            Next
        Next

        For i = 0 To UBound(tabBIN) Step 16
            If i Mod 10000 = 0 Then
                Console.Title = "trie #5 @ " & Format(i / UBound(tabBIN), "0.00%") & " (" & i & " / " & UBound(tabBIN) & ")"
            End If
            For j = 0 To UBound(tabBIN) Step 16
                If tabBIN(i) = tabBIN(j) And tabBIN(i + 1) = tabBIN(j + 1) And tabBIN(i + 2) = tabBIN(j + 2) And tabBIN(i + 3) = tabBIN(j + 3) And tabBIN(i + 4) < tabBIN(j + 4) Then
                    For k = 0 To UBound(tampon)
                        tampon(k) = tabBIN(j + k)
                        tabBIN(j + k) = tabBIN(i + k)
                        tabBIN(i + k) = tampon(k)
                    Next
                End If
            Next
        Next

        For i = 0 To UBound(tabBIN) Step 16
            If i Mod 10000 = 0 Then
                Console.Title = "trie #6 @ " & Format(i / UBound(tabBIN), "0.00%") & " (" & i & " / " & UBound(tabBIN) & ")"
            End If
            For j = 0 To UBound(tabBIN) Step 16
                If tabBIN(i) = tabBIN(j) And tabBIN(i + 1) = tabBIN(j + 1) And tabBIN(i + 2) = tabBIN(j + 2) And tabBIN(i + 3) = tabBIN(j + 3) And tabBIN(i + 4) = tabBIN(j + 4) And tabBIN(i + 5) < tabBIN(j + 5) Then
                    For k = 0 To UBound(tampon)
                        tampon(k) = tabBIN(j + k)
                        tabBIN(j + k) = tabBIN(i + k)
                        tabBIN(i + k) = tampon(k)
                    Next
                End If
            Next
        Next

        For i = 0 To UBound(tabBIN) Step 16
            If i Mod 10000 = 0 Then
                Console.Title = "trie #7 @ " & Format(i / UBound(tabBIN), "0.00%") & " (" & i & " / " & UBound(tabBIN) & ")"
            End If
            For j = 0 To UBound(tabBIN) Step 16
                If tabBIN(i) = tabBIN(j) And tabBIN(i + 1) = tabBIN(j + 1) And tabBIN(i + 2) = tabBIN(j + 2) And tabBIN(i + 3) = tabBIN(j + 3) And tabBIN(i + 4) = tabBIN(j + 4) And tabBIN(i + 5) = tabBIN(j + 5) And tabBIN(i + 6) < tabBIN(j + 6) Then
                    For k = 0 To UBound(tampon)
                        tampon(k) = tabBIN(j + k)
                        tabBIN(j + k) = tabBIN(i + k)
                        tabBIN(i + k) = tampon(k)
                    Next
                End If
            Next
        Next

        For i = 0 To UBound(tabBIN) Step 16
            If i Mod 10000 = 0 Then
                Console.Title = "trie #8 @ " & Format(i / UBound(tabBIN), "0.00%") & " (" & i & " / " & UBound(tabBIN) & ")"
            End If
            For j = 0 To UBound(tabBIN) Step 16
                If tabBIN(i) = tabBIN(j) And tabBIN(i + 1) = tabBIN(j + 1) And tabBIN(i + 2) = tabBIN(j + 2) And tabBIN(i + 3) = tabBIN(j + 3) And tabBIN(i + 4) = tabBIN(j + 4) And tabBIN(i + 5) = tabBIN(j + 5) And tabBIN(i + 6) = tabBIN(j + 6) And tabBIN(i + 7) < tabBIN(j + 7) Then
                    For k = 0 To UBound(tampon)
                        tampon(k) = tabBIN(j + k)
                        tabBIN(j + k) = tabBIN(i + k)
                        tabBIN(i + k) = tampon(k)
                    Next
                End If
            Next
        Next

        For i = 0 To UBound(tabBIN) Step 16
            If i Mod 10000 = 0 Then
                Console.Title = "trie #9 @ " & Format(i / UBound(tabBIN), "0.00%") & " (" & i & " / " & UBound(tabBIN) & ")"
            End If
            For j = 0 To UBound(tabBIN) Step 16
                If tabBIN(i) = tabBIN(j) And tabBIN(i + 1) = tabBIN(j + 1) And tabBIN(i + 2) = tabBIN(j + 2) And tabBIN(i + 3) = tabBIN(j + 3) And tabBIN(i + 4) = tabBIN(j + 4) And tabBIN(i + 5) = tabBIN(j + 5) And tabBIN(i + 6) = tabBIN(j + 6) And tabBIN(i + 7) = tabBIN(j + 7) And tabBIN(i + 8) < tabBIN(j + 8) Then
                    For k = 0 To UBound(tampon)
                        tampon(k) = tabBIN(j + k)
                        tabBIN(j + k) = tabBIN(i + k)
                        tabBIN(i + k) = tampon(k)
                    Next
                End If
            Next
        Next

        For i = 0 To UBound(tabBIN) Step 16
            If i Mod 10000 = 0 Then
                Console.Title = "trie #10 @ " & Format(i / UBound(tabBIN), "0.00%") & " (" & i & " / " & UBound(tabBIN) & ")"
            End If
            For j = 0 To UBound(tabBIN) Step 16
                If tabBIN(i) = tabBIN(j) And tabBIN(i + 1) = tabBIN(j + 1) And tabBIN(i + 2) = tabBIN(j + 2) And tabBIN(i + 3) = tabBIN(j + 3) And tabBIN(i + 4) = tabBIN(j + 4) And tabBIN(i + 5) = tabBIN(j + 5) And tabBIN(i + 6) = tabBIN(j + 6) And tabBIN(i + 7) = tabBIN(j + 7) And tabBIN(i + 8) = tabBIN(j + 8) And tabBIN(i + 9) < tabBIN(j + 9) Then
                    For k = 0 To UBound(tampon)
                        tampon(k) = tabBIN(j + k)
                        tabBIN(j + k) = tabBIN(i + k)
                        tabBIN(i + k) = tampon(k)
                    Next
                End If
            Next
        Next
        i = i - 1
        Console.Title = "trie #10 @ " & Format(i / UBound(tabBIN), "0.00%") & " (" & i & " / " & UBound(tabBIN) & ")"

        My.Computer.FileSystem.WriteAllBytes(cheminBIN1, tabBIN, False)
        My.Computer.FileSystem.DeleteFile(cheminBIN2)

        Console.Title = memo
    End Sub
End Module
