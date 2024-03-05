function start()
    this.PlayAnimation(1)
    
    if not IsDayEven() or not Random.OneIn(4) then
        this.GameObject.SetActive(false)
        return
    end
end

function interact()
    this.LogGraphContribution(0, 5)
end