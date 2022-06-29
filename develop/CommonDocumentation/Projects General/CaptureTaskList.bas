Attribute VB_Name = "CaptureTaskList"
Public Sub Capture(page As page)

    Dim shape As shape

    For Each shape In page.Shapes
    
        Debug.Print shape.Text
    
    Next

End Sub
