function start()
    if IsDayEven() then
        this.GameObject.SetActive(false)
        return
    end
end

function interact()
    this.LogGraphContribution(0, -4)
end