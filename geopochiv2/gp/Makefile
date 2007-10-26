ERL = erl -boot start_clean 
MODS = gp_root gp_data gp_main gp_index gp_session gp_util gp_geo gp_image

.SUFFIXES: .erl .beam

.erl.beam: 
	erlc -W $<

all: compile

compile: ${MODS:%=%.beam}

${MODS:%=%.beam}: gp_struct.hrl

clean:	
	rm -rf *.beam erl_crash.dump
