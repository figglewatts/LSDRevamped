function start()
    if not IsDayEven() or not Random.OneIn(4) then
        this.GameObject.SetActive(false)
        return
    end

    this.PlayAnimation(1)
end

function interact()
    this.LogGraphContribution(0, 5)
end