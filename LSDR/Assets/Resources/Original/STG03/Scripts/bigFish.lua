function start()
    if not IsDayLinear(3, 1) then
        this.GameObject.SetActive(false)
        return
    end

    this.PlayAnimation(0)
end

function interact()
    this.LogGraphContribution(6, 0)
end