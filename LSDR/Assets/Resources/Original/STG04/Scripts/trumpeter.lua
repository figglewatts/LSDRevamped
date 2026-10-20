function start()
    if IsDayEven() then
        this.GameObject.SetActive(false)
        return
    end

    this.PlayAnimation(0)
end

function interact()
    this.LogGraphContribution(2, 1)
end