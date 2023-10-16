function start()
    if not IsDayEven() then
        this.GameObject.SetActive(false)
        return
    end
end


function interact()
    this.LogGraphContribution(-7, 6)
end