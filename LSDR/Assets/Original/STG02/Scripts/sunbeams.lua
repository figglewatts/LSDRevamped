function start()
    if IsDayEven() then
        this.GameObject.SetActive(false)
        return
    end

    this.PlayAnimation(0)
end

function interact()
    DreamSystem.LogGraphContributionFromEntity(1, 0)
end