function start()
    this.PlayAnimation(0)
    
    if IsDayEven() then
        this.GameObject.SetActive(false)
        return
    end
end

function interact()
    this.LogGraphContribution(5, 3)
end