local mysql_center=require("database/mysql_auth_center");
local Response=require("Response");
local Stype=require("Stype");
local Cmd=require("Cmd");


--{stype,ctype,utag,body}
function do_edit_profile( s,req )
    local uid = req[3]
	local edit_profile_req = req[4]


	print(uid, edit_profile_req.unick, edit_profile_req.uface, edit_profile_req.usex)
    
    -- 参数错误
	if (string.len(edit_profile_req.unick) <= 0) or 
	   (edit_profile_req.uface <= 0 or edit_profile_req.uface > 9) or 
	   (edit_profile_req.usex ~= 0 and edit_profile_req.usex ~= 1) then 
		local msg = {Stype.Auth, Cmd.eEditProfileRes, uid, {
			status = Response.InvalidParams,
		}}

		Session.send_msg(s, msg)
		return
	end	

	-- 更新用户中心数据库
	local ret_handler = function(err, res)
		local ret_status = Response.OK
		if err ~= nil then
			ret_status = Response.SystemErr
		end

		--
		local msg = {Stype.Auth, Cmd.eEditProfileRes, uid, {
			status = ret_status,
		}}
		Session.send_msg(s, msg)
	end

    mysql_center.edit_profile(uid, 
                                edit_profile_req.unick,
                                edit_profile_req.uface, 
                                edit_profile_req.usex, 
                                ret_handler)
	--  end
end

local edit_profile={
    do_edit_profile=do_edit_profile,
}

return edit_profile;