function start()
    if IsDayEven() or (this.GameObject.Name ~= "BlinkingLight" and not IsDayLinear(3, 1)) then
        this.GameObject.SetActive(false)
        return
    end
end

function interact()
    this.LogGraphContribution(0, -9)
end