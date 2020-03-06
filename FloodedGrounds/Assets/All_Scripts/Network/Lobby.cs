public class Lobby
{
    private int port;
    private string name;
    private string ownerName;
    private int privacy;
    private bool passwordRequired;
    private string password;

    public Lobby()
    {
    }

    public Lobby(int port, string name, string ownerName, int privacy, bool passwordRequired, string password)
    {
        this.port = port;
        this.name = name;
        this.ownerName = ownerName;
        this.privacy = privacy;
        this.passwordRequired = passwordRequired;
        this.password = password;
    }

    public int getPort()
    {
        return this.port;
    }

    public string getName()
    {
        return this.name;
    }

    public string getOwnerName()
    {
        return this.ownerName;
    }

    public int getPrivacy()
    {
        return this.privacy;
    }

    public bool getPasswordRequired()
    {
        return this.passwordRequired;
    }

    public string getPassword()
    {
        return this.password;
    }

    public void setPort(int port)
    {
        this.port = port; ;
    }

    public void setName(string name)
    {
        this.name = name;
    }

    public void setOwnerName(string ownerName)
    {
        this.ownerName = ownerName;
    }

    public void setPrivacy(int privacy)
    {
        this.privacy = privacy;
    }

    public void setPasswordRequired(bool passwordRequired)
    {
        this.passwordRequired = passwordRequired;
    }
    
    public void setPassword(string password)
    {
        this.password = password;
    }
}
