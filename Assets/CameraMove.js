	

    var CamSpeed = 10;
    var GUIsize = 50;
     
    function Update () {
    var recdown = Rect (0, 0, Screen.width, GUIsize);
    var recup = Rect (0, Screen.height-GUIsize, Screen.width, GUIsize);
    var recleft = Rect (0, 0, GUIsize, Screen.height);
    var recright = Rect (Screen.width-GUIsize, 0, GUIsize, Screen.height);
     
        if (recdown.Contains(Input.mousePosition))
            transform.Translate(0,  (Time.deltaTime * -CamSpeed), 0, Space.World);
     
        if (recup.Contains(Input.mousePosition))
            transform.Translate(0,  (Time.deltaTime * CamSpeed), 0, Space.World);
     
        if (recleft.Contains(Input.mousePosition))
            transform.Translate((Time.deltaTime * -CamSpeed), 0, 0, Space.World);
     
        if (recright.Contains(Input.mousePosition))
            transform.Translate((Time.deltaTime * CamSpeed), 0, 0, Space.World);
    }

